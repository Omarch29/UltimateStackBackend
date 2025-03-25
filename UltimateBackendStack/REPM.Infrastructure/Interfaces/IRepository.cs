using REPM.Domain;

namespace REPM.Infrastructure.Interfaces;

public interface IRepository<TEntity> 
    where TEntity : BaseEntity
{
    IUnitOfWork UnitOfWork { get; }
        
    IQueryable<TEntity> Query { get; }
    IQueryable<TEntity> QueryToRead { get; }
    IQueryable<TEntity> QueryDeleted{ get; }
        
    Task<TEntity?> GetByIdAsync(Guid id);
    Task<List<TEntity?>> GetByIdRangeAsync(Guid[] ids);

    void Insert(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void DeletePermenantly(TEntity entity);
    
    void Delete(TEntity entity);
    
    void DeleteRange(IEnumerable<TEntity> entities);
    
    Task SaveChangesAsync();
}