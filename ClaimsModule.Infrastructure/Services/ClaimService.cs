using ClaimsModule.Application.Processors;
using ClaimsModule.Application.Repositories;
using ClaimsModule.Application.Services;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ClaimsModule.Infrastructure.Services;

/// <summary>
/// Handles the business logic for creating and storing insurance claims.
/// </summary>
public class ClaimService : IClaimService
{
    private readonly ILogger<ClaimService> _logger;

    private readonly IClaimRepository _claimRepository;
    private readonly IPolicyRepository _policyRepository;
    private readonly IServiceProvider _serviceProvider;
    //private readonly IDocumentRepository _documentRepository;
    //private readonly IFileStorageService _fileStorageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimService"/> class.
    /// </summary>
    /// <param name="claimRepository"></param>
    /// <param name="policyRepository"></param>
    /// <param name="documentRepository"></param>
    /// <param name="fileStorageService"></param>
    public ClaimService(IClaimRepository claimRepository, IPolicyRepository policyRepository,
        IServiceProvider serviceProvider, ILogger<ClaimService> logger)
    {
        _claimRepository = claimRepository;
        _policyRepository = policyRepository;
        //_documentRepository = documentRepository;
        //_fileStorageService = fileStorageService;
        _serviceProvider = serviceProvider;

        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Claim> CreateClaimAsync(Claim claim, IFormFileCollection? photos)
    {
        if (!await CheckPolicy(claim.PolicyId, claim.CustomerId))
        {
            throw new ArgumentException("The provided policy does not exist for this client.");
        }

        // Create claim entity
        Claim claimToCreate = new()
        {
            Id = Guid.NewGuid().ToString(),
            CustomerId = claim.CustomerId!,
            PolicyId = claim.PolicyId!,
            Description = claim.Description,
            SubmittedAt = DateTime.UtcNow,
            Status = ClaimStatus.Submitted
        };

        await _claimRepository.AddAsync(claimToCreate);

        //// TODO: Upload damage photos and save references
        //if (photos != null)
        //{
        //    foreach (IFormFile file in photos)
        //    {
        //        var url = await _fileStorageService.UploadAsync(file, claim.Id);
        //        var document = new Document(claim.Id, file.FileName, url);
        //        await _documentRepository.AddAsync(document);
        //    }
        //}

        _ = Task.Run(() => ProcessClaimAsync(claimToCreate.Id));

        return claimToCreate;
    }

    /// <inheritdoc/>
    public async Task<Claim?> GetClaimByIdAsync(string id)
    {
        return await _claimRepository.GetByIdAsync(id);
    }

    /// <inheritdoc/>
    public async Task ProcessClaimAsync(string claimId)
    {
        _logger.LogInformation("Started processing claim {ClaimId} at {Time}", claimId, DateTime.UtcNow);

        try
        {
            using var scope = _serviceProvider.CreateScope();

            // Resolve dependencies *inside this new scope*
            var decisionEngine = scope.ServiceProvider.GetRequiredService<IDecisionEngine>();
            var claimRepository = scope.ServiceProvider.GetRequiredService<IClaimRepository>();
            var docGenerator = scope.ServiceProvider.GetRequiredService<IDocumentGenerator>();

            Claim? existingClaim = await claimRepository.GetByIdAsync(claimId!);

            // Simulate semantic analysis step
            PolicyMatchResult policyMatchResult = new()
            {
                Id = Guid.NewGuid().ToString(),
                SimilarityScore = 1.0f,
            };
            existingClaim!.PolicyMatchResult = policyMatchResult;

            // Evaluate decision based on score
            Decision decision = decisionEngine.EvaluateClaim(existingClaim);
            existingClaim.Decision = decision;

            switch (decision.Type)
            {
                case DecisionType.Approved:
                    GeneratedDocument doc = docGenerator.GenerateAsync(existingClaim);
                    _logger.LogInformation("Document generated for approved claim {ClaimId} at {Path}", claimId, doc.FileUrl);
                    break;

                case DecisionType.Escalated:
                case DecisionType.Rejected:
                    _logger.LogInformation("No document generation required for decision {DecisionType}", decision.Type);
                    break;
            }

            // TODO: actual semantic analysis, similarity score, decision logic
            _logger.LogInformation("Successfully processed claim {ClaimId}", claimId);

            try
            {
                await claimRepository.UpdateAsync(existingClaim);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could update the claim");
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process claim {ClaimId}", claimId);
        }
    }

    /// <summary>
    /// Checks if the given policy belongs to the given client
    /// </summary>
    /// <param name="policyId">The id of the policy to be check against the clients polices.</param>
    /// <returns>True is the policy belongs to the client, False otherwise.</returns>
    private async Task<bool> CheckPolicy(string? policyId, string? clientId)
    {
        Policy? retrievedPolicy = await _policyRepository.GetByIdAsync(policyId!);
        if (retrievedPolicy is not null)
        {
            if(retrievedPolicy.ClientId == clientId)
            {
                return true;
            }
        }

        return false;
    }
}