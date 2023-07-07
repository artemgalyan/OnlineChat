using Database;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BusinessLogic.Repository;

public interface IUserRepository : IRepository<User, Guid>
{
    public Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken = default);
    public Task<bool> ContainsAllAsync(IReadOnlyCollection<Guid> userIds, CancellationToken cancellationToken = default);
}

public class UserRepository : IUserRepository
{
    private readonly ChatDatabase _chatDatabase;

    public UserRepository(ChatDatabase chatDatabase)
    {
        _chatDatabase = chatDatabase;
    }

    public Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _chatDatabase.Users.ToListAsync(cancellationToken);
    }

    public Task<User?> GetByIdAsync(Guid key, CancellationToken cancellationToken = default)
    {
        return _chatDatabase.Users
                            .FindAsync(new object?[] { key }, cancellationToken: cancellationToken)
                            .AsTask();
    }

    public Task<EntityEntry<User>> InsertAsync(User entity, CancellationToken cancellationToken = default)
    {
        return _chatDatabase.Users.AddAsync(entity, cancellationToken).AsTask();
    }

    public Task<int> DeleteAsync(Guid entityKey, CancellationToken cancellationToken = default)
    {
        return _chatDatabase.Users
                            .Where(u => u.Id == entityKey)
                            .ExecuteDeleteAsync(cancellationToken);
    }

    public Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
    {
        _chatDatabase.Users.Update(entity);
        return Task.CompletedTask;
    }

    public Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        return _chatDatabase.Users
                            .Where(u => u.Login == login)
                            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ContainsAllAsync(IReadOnlyCollection<Guid> userIds, CancellationToken cancellationToken = default)
    {
        return await _chatDatabase.Users
                                  .Where(u => userIds.Any(i => i == u.Id))
                                  .CountAsync(cancellationToken) == userIds.Count;
    }
}