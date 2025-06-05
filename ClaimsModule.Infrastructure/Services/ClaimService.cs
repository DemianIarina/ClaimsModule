using ClaimsModule.Application.Filters;
using ClaimsModule.Application.Processors;
using ClaimsModule.Application.Repositories;
using ClaimsModule.Application.Services;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using ClaimsModule.Infrastructure.Processors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
    public async Task<Claim> CreateClaimAsync(string policyId, string customerId, DateTime incidentTimestamp,
        string description, IFormFileCollection? photos)
    {
        Policy? retrievedPolicy = await _policyRepository.GetByIdAsync(policyId!);

        // Create claim entity
        Claim claimToCreate = new()
        {
            Id = Guid.NewGuid().ToString(),
            IncidentTimestamp = incidentTimestamp,
            Policy = retrievedPolicy,
            Description = description,
            SubmittedAt = DateTime.UtcNow,
            Status = ClaimStatus.Submitted,
            AssignedEmployee = retrievedPolicy!.ResponsibleEmployee
        };

        if(!ValidateClaimMetadata(retrievedPolicy, claimToCreate, customerId))
        {
            throw new ArgumentException("The claim is not valid for policy {PolicyId}. Could not create.", policyId);
        }

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

        _ = Task.Run(() => ProcessClaimAutomaticallyAsync(claimToCreate.Id));

        return claimToCreate;
    }

    /// <inheritdoc/>
    public async Task<List<Claim>> GetClaimsByCustomerAsync(string customerId)
        => await GetFilteredClaims(new ClaimFilter { CustomerId = customerId });

    /// <inheritdoc/>
    public async Task<List<Claim>> GetClaimsByEmpoyeeAsync(string employeeId)
        => await GetFilteredClaims(new ClaimFilter { EmployeeId = employeeId });

    private async Task<List<Claim>> GetFilteredClaims(ClaimFilter filter)
    {
        _logger.LogTrace("Fetching claims for filter {Filter}", filter);
        List<Claim> claims = await _claimRepository.GetListAsync(filter);
        _logger.LogTrace("Found {ClaimCount} claims for filter {Filter}", claims.Count, filter);
        return claims;
    }

    /// <inheritdoc/>
    public async Task<Claim?> GetClaimByIdAsync(string id)
    {
        return await _claimRepository.GetByIdAsync(id);
    }

    public async Task ProcessClaimAutomaticallyAsync(string claimId)
    {
        _logger.LogInformation("Started processing claim {ClaimId} at {Time}", claimId, DateTime.UtcNow);
        
        // Resolve dependencies *inside this new scope*
        using var scope = _serviceProvider.CreateScope();
        var decisionEngine = scope.ServiceProvider.GetRequiredService<IDecisionEngine>();
        var claimRepository = scope.ServiceProvider.GetRequiredService<IClaimRepository>();
        IDocumentGenerator documentGenerator = scope.ServiceProvider.GetRequiredService<IDocumentGenerator>();

        Claim? claim = await claimRepository.GetByIdAsync(claimId!);

        if (claim == null)
        {
            _logger.LogDebug("Claim not found for ID: {ClaimId}", claimId);
            throw new KeyNotFoundException($"Claim {claimId} not found");
        }

        // TODO: actual semantic analysis, similarity score, decision logic
        PolicyMatchResult policyMatchResult = new()
        {
            Id = Guid.NewGuid().ToString(),
            SimilarityScore = 0.5f,
        };
        claim!.PolicyMatchResult = policyMatchResult;

        // Evaluate decision based on score
        Decision decision = decisionEngine.EvaluateClaim(claim);
        claim.Decision = decision;

        await FinaliseClaimPorcessing(claim, claimRepository, documentGenerator);
    }

    public async Task ProcessClaimManuallyAsync(string claimId, bool approved, string employeeId)
    {
        // Resolve dependencies *inside this new scope*
        using var scope = _serviceProvider.CreateScope();
        var claimRepository = scope.ServiceProvider.GetRequiredService<IClaimRepository>();
        IDocumentGenerator documentGenerator = scope.ServiceProvider.GetRequiredService<IDocumentGenerator>();

        Claim? claim = await claimRepository.GetByIdAsync(claimId!);

        if (claim == null)
        {
            _logger.LogDebug("Claim not found for ID: {ClaimId}", claimId);
            throw new KeyNotFoundException($"Claim {claimId} not found");
        }

        if (claim.AssignedEmployee is null || claim.AssignedEmployee.Id != employeeId)
        {
            _logger.LogDebug("Employee {EmployeeId} attempted to evaluate claim {ClaimId} not assigned to them.", employeeId, claimId);
            throw new UnauthorizedAccessException($"Not authorized to evaluate claim {claimId}.");
        }

        IDecisionEngine decisionEngine = new ManualDecisionEngine(claim.AssignedEmployee.Name, approved);
        claim!.Decision = decisionEngine.EvaluateClaim(claim);

        await FinaliseClaimPorcessing(claim, claimRepository, documentGenerator);
    }

    /// <inheritdoc/>
    private async Task FinaliseClaimPorcessing(Claim claim, IClaimRepository claimRepository, IDocumentGenerator documentGenerator)
    {
        try
        {
            switch (claim.Decision!.Type)
            {
                case DecisionType.Approved:
                    GeneratedDocument doc = documentGenerator.GenerateAsync(claim);
                    claim.GeneratedDocument = doc;
                    _logger.LogInformation("Document generated for approved claim {ClaimId} at {Path}", claim.Id, doc.FileUrl);
                    break;

                    //TODO: send email to employee, add claim to his responsability list, send email to client
                    // - daca fail uieste send u de email la employee/ mai degraba daca da fail update u la responsability list - LogWarning
                    // big problem, manual intervention needed, oricum update la claim (nu incurca nici daca nu se face, da macar sa tratam la fel)
                    // oricum faci update la status, sa se poata continua 
                    // - daca fail uieste send u la client - logDebug - dar trebe oricum sa si sa facem update la claim, ca data
                    // viitoare cand intra clientul in app, sa vada statusu
                case DecisionType.Escalated:
                    //TODO: send email to client
                    //  - daca fail uieste send u la client - logDebug - dar trebe oricum sa si sa facem update la claim, ca data
                    // viitoare cand intra clientul in app, sa vada statusu
                case DecisionType.Rejected:
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to process claim {ClaimId}", claim.Id);
        }

        try
        {
            await claimRepository.UpdateAsync(claim);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could update the claim {Claim}. Manual intervention will be needed.", claim);
            throw;
        }
    }

    /// <summary>
    /// Validates the metadata of a claim by ensuring it meets policy-related criteria.
    /// </summary>
    /// <param name="policy">The insurance policy associated with the claim.</param>
    /// <param name="claim">The claim containing metadata to be validated, such as the incident timestamp.</param>
    /// <param name="customerId">The id of the customer who crates the claim.</param>
    /// <returns>True if the claim's incident timestamp falls within the policy's validity period and the policy
    /// on which the claim is created belongs to the customer; otherwise, False</returns>
    private bool ValidateClaimMetadata(Policy? policy, Claim claim, string customerId)
    {
        if(policy is null)
        {
            return false;
        }

        bool isWithinValidityPeriod = claim.IncidentTimestamp >= policy.ValidFrom &&
            claim.IncidentTimestamp <= policy.ValidTo;

        bool isOwnedBy = string.Equals(policy.Customer!.Id, customerId);

        return isWithinValidityPeriod && isOwnedBy;
    }
}