﻿// <auto-generated />
using System;
using ClaimsModule.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ClaimsModule.Infrastructure.Migrations
{
    [DbContext(typeof(ClaimsDbContext))]
    [Migration("20250605171745_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("ClaimsModule.Domain.Entities.AppUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("LinkedCustomerId")
                        .HasColumnType("longtext");

                    b.Property<string>("LinkedEmployeeId")
                        .HasColumnType("longtext");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ClaimsModule.Domain.Entities.Claim", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("AssignedEmployeeId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("DecisionId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Description")
                        .HasMaxLength(2000)
                        .HasColumnType("varchar(2000)");

                    b.Property<string>("GeneratedDocumentId")
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime?>("IncidentTimestamp")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("PolicyId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("PolicyMatchResultId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime?>("SubmittedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("AssignedEmployeeId");

                    b.HasIndex("DecisionId")
                        .IsUnique();

                    b.HasIndex("GeneratedDocumentId")
                        .IsUnique();

                    b.HasIndex("PolicyId");

                    b.HasIndex("PolicyMatchResultId")
                        .IsUnique();

                    b.ToTable("Claims");
                });

            modelBuilder.Entity("ClaimsModule.Domain.Entities.Customer", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("varchar(150)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("varchar(150)");

                    b.HasKey("Id");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("ClaimsModule.Domain.Entities.Decision", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("DecidedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("DecidedBy")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Reason")
                        .HasColumnType("longtext");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Decision");
                });

            modelBuilder.Entity("ClaimsModule.Domain.Entities.Employee", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("ClaimsModule.Domain.Entities.PersistedDocument", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ClaimId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ContentType")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("FileName")
                        .HasColumnType("longtext");

                    b.Property<string>("FileUrl")
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");

                    b.HasKey("Id");

                    b.HasIndex("ClaimId");

                    b.ToTable("PersistedDocument");
                });

            modelBuilder.Entity("ClaimsModule.Domain.Entities.Policy", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("CarBrand")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("CarModel")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("CarPlateNumber")
                        .HasColumnType("longtext");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("FileUrl")
                        .HasColumnType("longtext");

                    b.Property<string>("PolicyNumber")
                        .HasColumnType("longtext");

                    b.Property<string>("ResponsibleEmployeeId")
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("ValidFrom")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("ValidTo")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("ResponsibleEmployeeId");

                    b.ToTable("Policies");
                });

            modelBuilder.Entity("ClaimsModule.Domain.Entities.PolicyMatchResult", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<float?>("SimilarityScore")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("PolicyMatchResult");
                });

            modelBuilder.Entity("ClaimsModule.Domain.Entities.Claim", b =>
                {
                    b.HasOne("ClaimsModule.Domain.Entities.Employee", "AssignedEmployee")
                        .WithMany("AssignedClaims")
                        .HasForeignKey("AssignedEmployeeId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("ClaimsModule.Domain.Entities.Decision", "Decision")
                        .WithOne()
                        .HasForeignKey("ClaimsModule.Domain.Entities.Claim", "DecisionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ClaimsModule.Domain.Entities.PersistedDocument", "GeneratedDocument")
                        .WithOne()
                        .HasForeignKey("ClaimsModule.Domain.Entities.Claim", "GeneratedDocumentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ClaimsModule.Domain.Entities.Policy", "Policy")
                        .WithMany()
                        .HasForeignKey("PolicyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ClaimsModule.Domain.Entities.PolicyMatchResult", "PolicyMatchResult")
                        .WithOne()
                        .HasForeignKey("ClaimsModule.Domain.Entities.Claim", "PolicyMatchResultId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("AssignedEmployee");

                    b.Navigation("Decision");

                    b.Navigation("GeneratedDocument");

                    b.Navigation("Policy");

                    b.Navigation("PolicyMatchResult");
                });

            modelBuilder.Entity("ClaimsModule.Domain.Entities.PersistedDocument", b =>
                {
                    b.HasOne("ClaimsModule.Domain.Entities.Claim", null)
                        .WithMany("UploadedPhotos")
                        .HasForeignKey("ClaimId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ClaimsModule.Domain.Entities.Policy", b =>
                {
                    b.HasOne("ClaimsModule.Domain.Entities.Customer", "Customer")
                        .WithMany("Policies")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ClaimsModule.Domain.Entities.Employee", "ResponsibleEmployee")
                        .WithMany("Policies")
                        .HasForeignKey("ResponsibleEmployeeId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Customer");

                    b.Navigation("ResponsibleEmployee");
                });

            modelBuilder.Entity("ClaimsModule.Domain.Entities.Claim", b =>
                {
                    b.Navigation("UploadedPhotos");
                });

            modelBuilder.Entity("ClaimsModule.Domain.Entities.Customer", b =>
                {
                    b.Navigation("Policies");
                });

            modelBuilder.Entity("ClaimsModule.Domain.Entities.Employee", b =>
                {
                    b.Navigation("AssignedClaims");

                    b.Navigation("Policies");
                });
#pragma warning restore 612, 618
        }
    }
}
