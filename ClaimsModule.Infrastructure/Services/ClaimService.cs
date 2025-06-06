﻿using ClaimsModule.Application.Filters;
using ClaimsModule.Application.Processors;
using ClaimsModule.Application.Repositories;
using ClaimsModule.Application.RequestModels;
using ClaimsModule.Application.Services;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using ClaimsModule.Infrastructure.Config;
using ClaimsModule.Infrastructure.Processors;
using FileSignatures;
using FileSignatures.Formats;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
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
    private readonly IFileStorageService _fileStorageService;
    private readonly IEmailService _emailService;
    private readonly EmailTemplates _emailTemplates;
    private readonly IFileFormatInspector _inspector;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimService"/> class.
    /// </summary>
    /// <param name="claimRepository"></param>
    /// <param name="policyRepository"></param>
    /// <param name="fileStorageService"></param>
    public ClaimService(IClaimRepository claimRepository, IPolicyRepository policyRepository, IFileStorageService fileStorageService,
        IServiceProvider serviceProvider, IEmailService emailService, IOptions<EmailTemplates> emailTemplates, 
        IFileFormatInspector inspector, ILogger<ClaimService> logger)
    {
        _claimRepository = claimRepository;
        _policyRepository = policyRepository;
        _fileStorageService = fileStorageService;
        _serviceProvider = serviceProvider;
        _emailService = emailService;
        _emailTemplates = emailTemplates.Value;
        _inspector = inspector;

        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Claim> CreateClaimAsync(CreateClaimRequestModel createClaimRequestModel)
    {
        Policy? retrievedPolicy = await _policyRepository.GetByIdAsync(createClaimRequestModel.PolicyId);

        if(!ValidateClaimMetadata(retrievedPolicy, createClaimRequestModel))
        {
            throw new ArgumentException("The claim is not valid for policy {PolicyId}. Could not create.", createClaimRequestModel.PolicyId);
        }

        Claim claimToCreate = new()
        {
            Id = Guid.NewGuid().ToString(),
            IncidentTimestamp = createClaimRequestModel.IncidentDateTime,
            IncidentLocation = createClaimRequestModel.IncidentLocation,
            DamageType = createClaimRequestModel.DamageType,
            WasAnyoneInjured = createClaimRequestModel.WasAnyoneInjured,
            AreasDamaged = createClaimRequestModel.AreasDamaged,
            Policy = retrievedPolicy,
            Description = createClaimRequestModel.NarrativeText,
            SubmittedAt = DateTime.UtcNow,
            Status = ClaimStatus.Submitted,
            AssignedEmployee = retrievedPolicy!.ResponsibleEmployee
        };

        if (createClaimRequestModel.Photos != null && createClaimRequestModel.Photos.Count > 0)
        {
            claimToCreate.UploadedPhotos = new List<PersistedDocument>();

            foreach (IFormFile photo in createClaimRequestModel.Photos)
            {
                using Stream stream = photo.OpenReadStream();

                ValidateMimeType(stream, photo.FileName);

                string objectName = $"{claimToCreate.Id}/{Guid.NewGuid()}_{photo.FileName}";
                
                stream.Seek(0, SeekOrigin.Begin);
                string url = await _fileStorageService.UploadAsync(stream, objectName, photo.ContentType);

                claimToCreate.UploadedPhotos.Add(new PersistedDocument
                {
                    OriginalFileName = photo.FileName,
                    GeneratedFileName = objectName,
                    ContentType = photo.ContentType,
                    FileUrl = url
                });
            }
        }

        await _claimRepository.AddAsync(claimToCreate);

        // trigger background processing
        _ = Task.Run(() => ProcessClaimAutomaticallyAsync(claimToCreate.Id));

        return claimToCreate;
    }

    /// <inheritdoc/>
    public async Task<List<Claim>> GetClaimsByCustomerAsync(string customerId, string? policyId = null)
        => await GetFilteredClaims(new ClaimFilter
        {
            CustomerId = customerId,
            PolicyId = policyId
        });

    /// <inheritdoc/>
    public async Task<List<Claim>> GetClaimsByEmpoyeeAsync(string employeeId)
        => await GetFilteredClaims(new ClaimFilter { EmployeeId = employeeId, Status = ClaimStatus.Escalated });

    private async Task<List<Claim>> GetFilteredClaims(ClaimFilter filter)
    {
        _logger.LogTrace("Fetching claims for filter {Filter}", filter);
        List<Claim> claims = await _claimRepository.GetListAsync(filter);
        _logger.LogTrace("Found {ClaimCount} claims for filter {Filter}", claims.Count, filter);

        foreach (Claim claim in claims)
        {
            await RefreshPresignedUrls(claim);
        }

        return claims;
    }

    /// <inheritdoc/>
    public async Task<Claim?> GetClaimByIdAsync(string id)
    {
        Claim? claim = await _claimRepository.GetByIdAsync(id);

        if (claim != null)
        {
            await RefreshPresignedUrls(claim);
        }

        return claim;
    }

    /// <summary>
    /// Evaluates automatically and informs the customer and the employee of the status of a <see cref="Claim"/> 
    /// </summary>
    /// <param name="claimId">The Id of the <see cref="Claim"/> that needs to be processed</param>
    /// <exception cref="KeyNotFoundException"> When no <see cref="Claim"/> exists with the given Id.</exception>
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
                    PersistedDocument doc = await documentGenerator.GenerateAsync(claim);
                    claim.GeneratedDocument = doc;
                    _logger.LogInformation("Document generated for approved claim {ClaimId} at {Path}", claim.Id, doc.FileUrl);

                    await SendNotificationEmail(claim, claim.Policy!.Customer!.Email, _emailTemplates.ClaimApprovedSubject, _emailTemplates.ClaimApprovedBody, "Approval");

                    claim.Status = ClaimStatus.Approved;
                    break;
                case DecisionType.Escalated:
                    await SendNotificationEmail(claim, claim.Policy!.Customer!.Email, _emailTemplates.ClaimEscalatedSubject, _emailTemplates.ClaimEscalatedBody, "Escalation");
                    await SendNotificationEmail(claim, claim.Policy!.ResponsibleEmployee!.Email, _emailTemplates.ClaimEscalationAssignedSubject, _emailTemplates.ClaimEscalationAssignedBody, "EscalationToEmployee");

                    claim.Status = ClaimStatus.Escalated;
                    break;
                case DecisionType.Rejected:
                    await SendNotificationEmail(claim, claim.Policy!.Customer!.Email,_emailTemplates.ClaimRejectedSubject, _emailTemplates.ClaimRejectedBody, "Rejection");

                    claim.Status = ClaimStatus.Rejected;
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
    /// Validates whether a claim request metadata aligns with the policy. constrains.
    /// It ensures:
    /// - The policy exists.
    /// - The incident date is within the policy validity window.
    /// - The policy belongs to the customer submitting the claim.
    /// - The incident timestamp is not in the future.
    /// </summary>
    /// <param name="policy">The insurance policy associated with the claim.</param>
    /// <param name="createClaimRequestModel"></param>
    private bool ValidateClaimMetadata(Policy? policy, CreateClaimRequestModel createClaimRequestModel)
    {
        if (policy is null)
        {
            return false;
        }

        DateTime now = DateTime.Now;

        if (createClaimRequestModel.IncidentDateTime > now)
            return false;

        if (createClaimRequestModel.IncidentDateTime < policy.ValidFrom || createClaimRequestModel.IncidentDateTime > policy.ValidTo)
            return false;

        if (!string.Equals(policy.Customer?.Id, createClaimRequestModel.CustomerId))
            return false;

        return true;
    }

    private async Task SendNotificationEmail(Claim claim, string recipientEmail, string subjectTemplate, string bodyTemplate, string statusLabelForLog)
    {
        try
        {
            string subject = subjectTemplate
                .Replace("{PolicyNumber}", claim.Policy!.PolicyNumber!)
                .Replace("{IncidentDate}", claim.IncidentTimestamp!.Value.ToString("yyyy-MM-dd"));

            string body = bodyTemplate
                .Replace("{CustomerName}", claim.Policy.Customer!.Name)
                .Replace("{EmployeeName}", claim.Policy.ResponsibleEmployee!.Name)
                .Replace("{PolicyNumber}", claim.Policy.PolicyNumber!)
                .Replace("{IncidentDate}", claim.IncidentTimestamp!.Value.ToString("yyyy-MM-dd"));

            await _emailService.SendEmailAsync(recipientEmail, subject, body);
            _logger.LogTrace("{StatusLabel} email sent to {RecipientEmail} for claim under Policy {PolicyNumber}", statusLabelForLog, recipientEmail, claim.Policy.PolicyNumber);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to send {StatusLabel} email to {RecipientEmail} for claim under Policy {PolicyNumber}", statusLabelForLog, recipientEmail, claim.Policy!.PolicyNumber);
        }
    }

    private async Task RefreshPresignedUrls(Claim claim)
    {
        if(claim.UploadedPhotos is not null)
        {
            foreach (PersistedDocument photo in claim.UploadedPhotos)
            {
                photo.FileUrl = await _fileStorageService.GeneratePresignedUrlAsync(photo.GeneratedFileName!);
            }
        }

        if (claim.GeneratedDocument != null)
        {
            claim.GeneratedDocument.FileUrl = await _fileStorageService.GeneratePresignedUrlAsync(claim.GeneratedDocument.GeneratedFileName!);
        }
    }

    /// <summary>
    /// Validate if a file has one of the accepted types
    /// </summary>
    /// <param name="fileStream">Stream of the file that will be validated</param>
    /// <exception cref="InvalidOperationException">Thrown if the file</exception>
    private void ValidateMimeType(Stream fileStream, string fileName)
    {
        FileFormat? format = _inspector.DetermineFileFormat(fileStream);

        if(!(format is Image || format is Pdf))
        {
            throw new InvalidOperationException($"File {fileName} has a type that is not allowed. Accepted types: Image or PDF");
        }
    }
}