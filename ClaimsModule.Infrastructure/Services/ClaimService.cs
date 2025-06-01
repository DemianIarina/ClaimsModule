using ClaimsModule.Application.Repositories;
using ClaimsModule.Application.Services;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using Microsoft.AspNetCore.Http;
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
    //private readonly IDocumentRepository _documentRepository;
    //private readonly IFileStorageService _fileStorageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimService"/> class.
    /// </summary>
    /// <param name="claimRepository"></param>
    /// <param name="policyRepository"></param>
    /// <param name="documentRepository"></param>
    /// <param name="fileStorageService"></param>
    public ClaimService(IClaimRepository claimRepository, IPolicyRepository policyRepository, ILogger<ClaimService> logger)
    {
        _claimRepository = claimRepository;
        _policyRepository = policyRepository;
        //_documentRepository = documentRepository;
        //_fileStorageService = fileStorageService;

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
            Id = Guid.NewGuid(),
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

        return claimToCreate;
    }

    /// <inheritdoc/>
    public async Task<Claim?> GetClaimByIdAsync(string id)
    {
        return await _claimRepository.GetByIdAsync(id);
    }

    /// <inheritdoc/>
    public async Task ProcessClaimAsync(Claim claim)
    {
        _logger.LogInformation("Started processing claim {ClaimId} at {Time}", claim.Id, DateTime.UtcNow);

        try
        {
            // Simulate background processing
            await Task.Delay(2000);

            // TODO: actual semantic analysis, similarity score, decision logic
            _logger.LogInformation("Successfully processed claim {ClaimId}", claim.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process claim {ClaimId}", claim.Id);
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