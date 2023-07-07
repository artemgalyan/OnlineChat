using Database;
using Entities.Chatrooms.PublicChatroom;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BusinessLogic.Repository;

public interface IAdministratorsRepository : IRepository<Administrators, Guid>
{
    public Task<Administrators?> GetByChatroomId(Guid chatId, CancellationToken cancellationToken);
}

public class AdministratorsRepository : IAdministratorsRepository
{
    private readonly ChatDatabase _chatDatabase;

    public AdministratorsRepository(ChatDatabase chatDatabase)
    {
        _chatDatabase = chatDatabase;
    }

    public Task<List<Administrators>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _chatDatabase.Administrators.ToListAsync(cancellationToken);
    }

    public Task<Administrators?> GetByIdAsync(Guid key, CancellationToken cancellationToken = default)
    {
        return _chatDatabase.Administrators
                            .FindAsync(new object?[] { key }, cancellationToken)
                            .AsTask();
    }

    public Task<EntityEntry<Administrators>> InsertAsync(Administrators entity, CancellationToken cancellationToken = default)
    {
        return _chatDatabase.Administrators.AddAsync(entity, cancellationToken).AsTask();
    }

    public Task<int> DeleteAsync(Guid entityKey, CancellationToken cancellationToken = default)
    {
        return _chatDatabase.Administrators
                            .Where(a => a.Id == entityKey)
                            .ExecuteDeleteAsync(cancellationToken);

    }

    public Task UpdateAsync(Administrators entity, CancellationToken cancellationToken = default)
    {
        _chatDatabase.Administrators.Update(entity);
        return Task.CompletedTask;
    }

    public Task<Administrators?> GetByChatroomId(Guid chatId, CancellationToken cancellationToken)
    {
        return _chatDatabase.Administrators
                            .Where(a => a.PublicChatroomId == chatId)
                            .FirstOrDefaultAsync(cancellationToken);
    }
}