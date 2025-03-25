using REPM.Domain;

namespace REPM.Infrastructure.Interfaces;

public interface IRepository<TEntity> 
    where TEntity : BaseEntity
{
    IUnitOfWork UnitOfWork { get; }
        
    IQueryable<TEntity> Query { get; }
    IQueryable<TEntity> QueryToRead { get; }
    IQueryable<TEntity> QueryDeleted{ get; }
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdReadOnlyAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<TEntity?>> GetByIdRangeAsync(Guid[] ids, CancellationToken cancellationToken = default);

    void Insert(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void DeletePermenantly(TEntity entity);
    
    void Delete(TEntity entity);
    
    void DeleteRange(IEnumerable<TEntity> entities);
    
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
}