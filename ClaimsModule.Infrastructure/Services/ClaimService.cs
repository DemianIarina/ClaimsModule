using ClaimsModule.Application.Repositories;
using ClaimsModule.Application.Services;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace ClaimsModule.Infrastructure.Services;

/// <summary>
/// Handles the business logic for creating and storing insurance claims.
/// </summary>
public class ClaimService : IClaimService
{
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
    public ClaimService(
        IClaimRepository claimRepository, IPolicyRepository policyRepository)
    {
        _claimRepository = claimRepository;
        _policyRepository = policyRepository;
        //_documentRepository = documentRepository;
        //_fileStorageService = fileStorageService;
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

        //// Upload damage photos and save references
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="policyId"></param>
    /// <returns></returns>
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