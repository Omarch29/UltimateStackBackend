using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using REPM.Domain;
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
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var userId = _httpContextAccessor.HttpContext.User.GetUserId();

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
        
    }
}
