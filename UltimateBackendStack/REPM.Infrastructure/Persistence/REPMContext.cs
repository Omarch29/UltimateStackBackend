using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using REPM.Domain;
using REPM.Domain.Entities;
using REPM.Infrastructure.Interfaces;
using REPM.Infrastructure.Security;

namespace REPM.Infrastructure.Persistence;

public class REPMContext : DbContext, IUnitOfWork
{
    protected readonly IHttpContextAccessor _httpContextAccessor;

    public REPMContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Property> Properties { get; set; }
    public virtual DbSet<Lease> Leases { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }
    
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        // Get user ID from HTTP context if available, otherwise use a default value for seeding
        var userId = _httpContextAccessor.HttpContext?.User?.GetUserId() ?? Guid.Empty;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:

                    // Populate User Id on DbContext entity insert
                    if (entry.Entity.CreatedBy == default)
                        entry.Entity.CreatedBy = userId;

                    if (entry.Entity.UpdatedBy == default)
                        entry.Entity.UpdatedBy = userId;

                    if (entry.Entity.CreatedAt == default)
                        entry.Entity.SetCreatedAt(DateTime.UtcNow);

                    if (entry.Entity.UpdatedAt == default)
                        entry.Entity.SetUpdatedAt(DateTime.UtcNow);

                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedBy = userId;
                    entry.Entity.SetUpdatedAt(DateTime.UtcNow);

                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Property>(builder =>
        {
            builder.OwnsOne(p => p.Address, address =>
            {
                address.Property(a => a.Street);
                address.Property(a => a.City);
                address.Property(a => a.State);
                address.Property(a => a.ZipCode);
            });
            
            builder.HasOne(p => p.Owner)
                .WithMany(u => u.Properties)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<Lease>(builder =>
        {
            builder.HasOne(l => l.Property)
                .WithMany(p => p.Leases)
                .HasForeignKey(l => l.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(l => l.Tenant)
                .WithMany()
                .HasForeignKey(l => l.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.OwnsOne(l => l.LeasePeriod, period =>
            {
                period.Property(p => p.Start);
                period.Property(p => p.End);
            });
            
            builder.OwnsOne(l => l.RentAmount, amount =>
            {
                amount.Property(a => a.Amount);
                amount.Property(a => a.Currency);
            });
            
        });
        
        modelBuilder.Entity<User>(builder =>
        {
            builder.HasMany(u => u.Properties)
                .WithOne(p => p.Owner)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(u => u.Leases)
                .WithOne(l => l.Tenant)
                .HasForeignKey(l => l.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<Payment>(builder =>
        {
            builder.HasOne(p => p.Lease)
                .WithMany(l => l.Payments)
                .HasForeignKey(p => p.LeaseId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.OwnsOne(p => p.Amount, amount =>
            {
                amount.Property(a => a.Amount);
                amount.Property(a => a.Currency);
            });
        });
        
    }
}
