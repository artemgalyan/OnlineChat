using Database;
using Entities.Chatrooms.PublicChatroom;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BusinessLogic.Repository;

public interface IUserAdministratorRepository : IRepository<UserAdministrator, (Guid UserId, Guid AdministratorsId)>
{
}

public class UserAdministratorRepository : IUserAdministratorRepository
{
    private readonly ChatDatabase _chatDatabase;

    public UserAdministratorRepository(ChatDatabase chatDatabase)
    {
        _chatDatabase = chatDatabase;
    }

    public Task<List<UserAdministrator>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _chatDatabase.UserAdministrators.ToListAsync(cancellationToken);
    }

    public Task<UserAdministrator?> GetByIdAsync((Guid UserId, Guid AdministratorsId) key,
        CancellationToken cancellationToken = default)
    {
        return _chatDatabase.UserAdministrators
                            .FindAsync(new object?[] { key.UserId, key.AdministratorsId }, cancellationToken)
                            .AsTask();
    }

    public Task<EntityEntry<UserAdministrator>> InsertAsync(UserAdministrator entity,
        CancellationToken cancellationToken = default)
    {
        return _chatDatabase.UserAdministrators
                            .AddAsync(entity, cancellationToken)
                            .AsTask();
    }

    public Task<int> DeleteAsync((Guid UserId, Guid AdministratorsId) entityKey,
        CancellationToken cancellationToken = default)
    {
        return _chatDatabase.UserAdministrators
                            .Where(a => a.UserId == entityKey.UserId &&
                                        a.AdministratorsId == entityKey.AdministratorsId)
                            .ExecuteDeleteAsync(cancellationToken);
    }

    public Task UpdateAsync(UserAdministrator entity, CancellationToken cancellationToken = default)
    {
        _chatDatabase.UserAdministrators.Update(entity);
        return Task.CompletedTask;
    }
}