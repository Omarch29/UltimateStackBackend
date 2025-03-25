using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using REPM.Domain;
using REPM.Infrastructure.Interfaces;
using REPM.Infrastructure.Persistence;

namespace REPM.Infrastructure.Repositories;

public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected readonly REPMContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    protected readonly IHttpContextAccessor _httpContextAccessor;
    
    public Repository(REPMContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _dbSet = _context.Set<TEntity>();
    }

    public virtual IUnitOfWork UnitOfWork => _context;
    public IQueryable<TEntity> QueryToRead => _dbSet.AsNoTracking().Where(x => !x.IsDeleted);
    public IQueryable<TEntity> Query => _dbSet.Where(x => !x.IsDeleted);
    public IQueryable<TEntity> QueryDeleted => _dbSet.Where(x => x.IsDeleted);
    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
    
    public Task<TEntity?> GetByIdReadOnlyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return QueryToRead.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public virtual async Task<List<TEntity?>> GetByIdRangeAsync(Guid[] ids, CancellationToken cancellationToken = default)
    {
        return (await Query.Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken))!;
    }

    public void Insert(TEntity entity)
    {
        _dbSet.Add(entity);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        _dbSet.AddRange(entities);
    }

    public void DeletePermenantly(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public void Delete(TEntity entity)
    {
        entity.Delete();
    }

    public void DeleteRange(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.Delete();
        }
    }
    
    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return (await _context.SaveChangesAsync(cancellationToken)) > 0;
    }
    
}