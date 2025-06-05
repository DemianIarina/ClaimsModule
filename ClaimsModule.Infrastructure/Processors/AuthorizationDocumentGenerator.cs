using ClaimsModule.Application.Processors;
using ClaimsModule.Application.Services;
using ClaimsModule.Domain.Entities;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System;
using System.IO;
using Document = QuestPDF.Fluent.Document;

namespace ClaimsModule.Infrastructure.Processors;

public class AuthorizationDocumentGenerator : IDocumentGenerator
{
    private readonly ILogger<AuthorizationDocumentGenerator> _logger;
    private readonly IFileStorageService _fileStorage;

    public AuthorizationDocumentGenerator(ILogger<AuthorizationDocumentGenerator> logger, IFileStorageService fileStorage)
    {
        _logger = logger;
        _fileStorage = fileStorage;
    }

    public PersistedDocument GenerateAsync(Claim claim)
    {
        string fileName = $"Claim-{claim.Policy!.PolicyNumber}-{claim.SubmittedAt:yyyyMMddHHmmss}.pdf";
        string objectName = $"{claim.Id}/{fileName}";

        using var memoryStream = new MemoryStream();

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Caring Insurance").FontSize(20).Bold();
                        col.Item().Text("Repair Authorization Certificate").FontSize(14).SemiBold().ParagraphSpacing(5);
                    });

                    row.ConstantItem(100).Height(50).Placeholder(); // Logo Placeholder
                });

                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Spacing(8);

                    col.Item().Text($"Date of Issue: {DateTime.UtcNow:yyyy-MM-dd}").FontSize(10);

                    col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    col.Item().PaddingBottom(5);

                    // Policyholder Information
                    col.Item().Text("Policyholder Information").FontSize(12).Bold().ParagraphSpacing(10);
                    col.Item().Text($"Name: {claim.Policy!.Customer!.Name}");
                    col.Item().Text($"Contact: {claim.Policy.Customer.Email}");
                    col.Item().Text($"Policy Number: {claim.Policy.PolicyNumber}");
                    col.Item().Text($"Vehicle Plate Number: {claim.Policy.CarPlateNumber}");
                    col.Item().Text($"Car: {claim.Policy.CarBrand} {claim.Policy.CarModel}");

                    col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    col.Item().PaddingBottom(5);

                    // Claim Info
                    col.Item().Text("Claim Information").FontSize(12).Bold().ParagraphSpacing(10);
                    col.Item().Text($"Claim ID: {claim.Id}");
                    col.Item().Text($"Description: {claim.Description}");
                    col.Item().Text($"Submitted At: {claim.SubmittedAt:yyyy-MM-dd}");

                    col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    col.Item().PaddingBottom(5);

                    // Authorization Details
                    col.Item().Text("Authorization Details").FontSize(12).Bold().ParagraphSpacing(10);
                    col.Item().Text("Authorized Garage: Caring Auto Repair");
                    col.Item().Text("Valid For: 30 days from issuance");
                    col.Item().Text("Instructions: Present valid ID. Garage must invoice insurer.");

                    // Footer
                    page.Footer()
                        .AlignCenter()
                        .PaddingVertical(10)
                        .Column(col =>
                        {
                            col.Spacing(2);
                            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            col.Item().Text("Authorized by: Claims Department")
                                .SemiBold().FontSize(10);

                            col.Item().Text("Contact: support@caring-insurance.com")
                                .FontSize(10);

                            col.Item().Text("This document is generated electronically and valid without signature. Subject to verification under policy.")
                                .FontSize(8).Italic().FontColor(Colors.Grey.Medium);
                        });
                });
            });
        }).GeneratePdf(memoryStream);

        memoryStream.Seek(0, SeekOrigin.Begin);

        var fileUrl = _fileStorage.UploadAsync(memoryStream, objectName, "application/pdf").GetAwaiter().GetResult();

        _logger.LogInformation("PDF document generated at {Path}", fileUrl);

        return new PersistedDocument
        {
            Id = Guid.NewGuid().ToString(),
            FileName = fileName,
            FileUrl = fileUrl,
            ContentType = "application/pdf",
            CreatedAt = DateTime.UtcNow
        };
    }
}
