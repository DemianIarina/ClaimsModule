using ClaimsModule.Application.Processors;
using ClaimsModule.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;
using System;

namespace ClaimsModule.Infrastructure.Processors;

public class AuthorizationDocumentGenerator : IDocumentGenerator
{
    private readonly ILogger<AuthorizationDocumentGenerator> _logger;
    private const string OutputDir = "GeneratedDocs";

    public AuthorizationDocumentGenerator(ILogger<AuthorizationDocumentGenerator> logger)
    {
        _logger = logger;
        Directory.CreateDirectory(OutputDir);
    }

    public GeneratedDocument GenerateAsync(Claim claim)
    {
        string fileName = $"Claim-{claim.Id}.pdf";
        string fullPath = Path.Combine(OutputDir, fileName);

        GeneratedDocument document = new()
        {
            Id = Guid.NewGuid().ToString(),
            FileUrl = fullPath,
            CreatedAt = DateTime.Now
        };

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

                    col.Item().Text($"Document ID: {document.Id}").FontSize(10);
                    col.Item().Text($"Date of Issue: {DateTime.UtcNow:yyyy-MM-dd}").FontSize(10);

                    col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    col.Item().PaddingBottom(5);

                    // Policyholder Information
                    col.Item().Text("Policyholder Information").FontSize(12).Bold().ParagraphSpacing(10);
                    col.Item().Text($"Name: {claim.Policy!.Customer!.Name}");
                    col.Item().Text($"Contact: {claim.Policy.Customer.Email}");
                    col.Item().Text($"Policy ID: {claim.Policy.Id}");
                    col.Item().Text($"Vehicle Plate Number: {claim.Policy.CarPlateNumber}");

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
        }).GeneratePdf(fullPath);

        _logger.LogInformation("PDF document generated at {Path}", fullPath);

        return document;
    }
}
