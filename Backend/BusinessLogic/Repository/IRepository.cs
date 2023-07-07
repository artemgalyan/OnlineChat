using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BusinessLogic.Repository;

public interface IRepository<TEntity, in TKey> where TEntity : class
{
    public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<TEntity?> GetByIdAsync(TKey key, CancellationToken cancellationToken = default);
    public Task<EntityEntry<TEntity>> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
    public Task<int> DeleteAsync(TKey entityKey, CancellationToken cancellationToken = default);
    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
}