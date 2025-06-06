﻿using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Infrastructure;

/// <summary>
/// Represents the Entity Framework database context for managing insurance claim data.
/// </summary>
public class ClaimsDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimsDbContext"/> class with the specified options.
    /// </summary>
    /// <param name="options">The options to be used by the <see cref="DbContext"/>.</param>
    public ClaimsDbContext(DbContextOptions<ClaimsDbContext> options) : base(options) { }

    /// <summary>
    /// Set of <see cref="Claim"/> entities in the database.
    /// </summary>
    public DbSet<Claim> Claims => Set<Claim>();

    /// <summary>
    /// Set of <see cref="Policy"/> entities in the database.
    /// </summary>
    public DbSet<Policy> Policies => Set<Policy>();

    /// <summary>
    /// Set of <see cref="Customer"/> entities in the database.
    /// </summary>
    public DbSet<Customer> Customers => Set<Customer>();

    /// <summary>
    /// Set of <see cref="AppUser"/> entities in the database.
    /// </summary>
    public DbSet<AppUser> Users { get; set; }

    /// <summary>
    /// Set of <see cref="Employee"/> entities in the database
    /// </summary>
    public DbSet<Employee> Employees { get; set; }

    /// <summary>
    /// Configures the EF Core model using the fluent API.
    /// </summary>
    /// <param name="modelBuilder">The builder used to construct the model for the context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Add custom configs if needed

        modelBuilder.Entity<Claim>(entity =>
        {
            entity.HasKey(c => c.Id);

            // Many-to-One: Claim → Policy
            entity.HasOne(c => c.Policy)
              .WithMany() 
              .HasForeignKey("PolicyId")
              .IsRequired()
              .OnDelete(DeleteBehavior.Restrict);

            // One-to-one: Claim -> PolicyMatchResult
            entity.HasOne(c => c.PolicyMatchResult)
                  .WithOne()
                  .HasForeignKey<Claim>("PolicyMatchResultId")
                  .OnDelete(DeleteBehavior.Cascade);

            // One-to-one: Claim -> Decision
            entity.HasOne(c => c.Decision)
                  .WithOne()
                  .HasForeignKey<Claim>("DecisionId")
                  .OnDelete(DeleteBehavior.Cascade);

            // One-to-one: Claim -> GeneratedDocument
            entity.HasOne(c => c.GeneratedDocument)
                  .WithOne()
                  .HasForeignKey<Claim>("GeneratedDocumentId")
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(c => c.UploadedPhotos)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);

            // Many-to-One: Claim → Employee (AssignedEmployee)
            entity.HasOne(c => c.AssignedEmployee)
                  .WithMany(e => e.AssignedClaims)
                  .HasForeignKey("AssignedEmployeeId")
                  .OnDelete(DeleteBehavior.SetNull);

            // Optional: timestamps and string length constraints
            entity.Property(c => c.Description)
                  .HasMaxLength(2000);

            entity.Property(c => c.Status)
                  .HasMaxLength(100)
                  .IsRequired();
        });

        modelBuilder.Entity<PolicyMatchResult>(entity =>
        {
            entity.HasKey(p => p.Id);
        });

        modelBuilder.Entity<Decision>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Type).HasMaxLength(50).IsRequired();
            entity.Property(d => d.DecidedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<PersistedDocument>(entity =>
        {
            entity.HasKey(g => g.Id);
        });

        modelBuilder.Entity<Policy>(entity =>
        {
            entity.HasKey(p => p.Id);

            // Many-to-One: Policy → Customer
            entity.HasOne(p => p.Customer)
                  .WithMany(c => c.Policies)
                  .HasForeignKey("CustomerId")
                  .IsRequired()
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.ResponsibleEmployee)
              .WithMany(e => e.Policies)
              .HasForeignKey("ResponsibleEmployeeId")
              .IsRequired(false)
              .OnDelete(DeleteBehavior.SetNull);

            entity.Property(p => p.CarBrand).HasMaxLength(100);
            entity.Property(p => p.CarModel).HasMaxLength(100);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).HasMaxLength(150);
            entity.Property(c => c.Email).HasMaxLength(150);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
        });
    }
}