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
    //private readonly IDocumentRepository _documentRepository;
    //private readonly IFileStorageService _fileStorageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimService"/> class.
    /// </summary>
    /// <param name="claimRepository"></param>
    /// <param name="documentRepository"></param>
    /// <param name="fileStorageService"></param>
    public ClaimService(
        IClaimRepository claimRepository)
    {
        _claimRepository = claimRepository;
        //_documentRepository = documentRepository;
        //_fileStorageService = fileStorageService;
    }

    /// <inheritdoc />
    public async Task<Claim> CreateClaimAsync(Claim claim, IFormFileCollection? photos)
    {
        // TODO: Retrieve customer policyId
        string policyId = "policyId";

        // Create claim entity
        Claim claimToCreate = new()
        {
            Id = Guid.NewGuid(),
            CustomerId = claim.CustomerId!,
            PolicyId = policyId!,
            Description = claim.Description,
            SubmittedAt = DateTime.UtcNow,
            Status = ClaimStatus.Submitted
        };

        await _claimRepository.AddAsync(claim);

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

        return claim;
    }
}