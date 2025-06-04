using ClaimsModule.Domain.Entities;
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

        modelBuilder.Entity<GeneratedDocument>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.Property(g => g.FileUrl).HasMaxLength(500);
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

            entity.Property(p => p.CarBrand).HasMaxLength(100);
            entity.Property(p => p.CarModel).HasMaxLength(100);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).HasMaxLength(150);
            entity.Property(c => c.Email).HasMaxLength(150);
        });
    }
}