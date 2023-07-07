using Database;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BusinessLogic.Repository;

public interface IUserTokenRepository : IRepository<UserToken, string>
{
    public Task<int> DeleteByUserId(Guid userId, CancellationToken cancellationToken);
}

public class UserTokenRepository : IUserTokenRepository
{
    private readonly ChatDatabase _chatDatabase;

    public UserTokenRepository(ChatDatabase chatDatabase)
    {
        _chatDatabase = chatDatabase;
    }

    public Task<List<UserToken>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _chatDatabase.UserTokens.ToListAsync(cancellationToken);
    }

    public Task<UserToken?> GetByIdAsync(string key, CancellationToken cancellationToken = default)
    {
        return _chatDatabase.UserTokens
                            .FindAsync(key, cancellationToken)
                            .AsTask();
    }

    public Task<EntityEntry<UserToken>> InsertAsync(UserToken entity, CancellationToken cancellationToken = default)
    {
        return _chatDatabase.UserTokens
                            .AddAsync(entity, cancellationToken)
                            .AsTask();
    }

    public Task<int> DeleteAsync(string entityKey, CancellationToken cancellationToken = default)
    {
        return _chatDatabase.UserTokens
                            .Where(t => t.JwtToken == entityKey)
                            .ExecuteDeleteAsync(cancellationToken);

    }

    public Task UpdateAsync(UserToken entity, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<int> DeleteByUserId(Guid userId, CancellationToken cancellationToken)
    {
        return _chatDatabase.UserTokens
                     .Where(t => t.UserId == userId)
                     .ExecuteDeleteAsync(cancellationToken);
    }

    public Task<bool> ContainsToken()
    {
        throw new NotImplementedException();
    }
}