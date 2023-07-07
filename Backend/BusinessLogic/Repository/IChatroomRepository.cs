using Database;
using Entities.Chatrooms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BusinessLogic.Repository;

public interface IChatroomRepository : IRepository<Chatroom, Guid>
{
    public Task<PrivateChatroom?> GetPrivateChatroomByUsersAsync(Guid firstId, Guid secondId,
        CancellationToken cancellationToken = default);
}

public class ChatroomRepository : IChatroomRepository
{
    private readonly ChatDatabase _chatDatabase;

    public ChatroomRepository(ChatDatabase chatDatabase)
    {
        _chatDatabase = chatDatabase;
    }

    public Task<List<Chatroom>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _chatDatabase.Chatroom.ToListAsync(cancellationToken);
    }

    public Task<Chatroom?> GetByIdAsync(Guid key, CancellationToken cancellationToken = default)
    {
        return _chatDatabase.Chatroom
                            .FindAsync(new object?[] { key }, cancellationToken: cancellationToken)
                            .AsTask();
    }

    public Task<EntityEntry<Chatroom>> InsertAsync(Chatroom entity, CancellationToken cancellationToken = default)
    {
        return _chatDatabase.Chatroom
                            .AddAsync(entity, cancellationToken)
                            .AsTask();
    }

    public Task<int> DeleteAsync(Guid entityKey, CancellationToken cancellationToken = default)
    {
        return _chatDatabase.Chatroom
                            .Where(c => c.Id == entityKey)
                            .ExecuteDeleteAsync(cancellationToken);
    }

    public Task UpdateAsync(Chatroom entity, CancellationToken cancellationToken = default)
    {
        _chatDatabase.Chatroom.Update(entity);
        return Task.CompletedTask;
    }

    public Task<PrivateChatroom?> GetPrivateChatroomByUsersAsync(Guid firstId, Guid secondId,
        CancellationToken cancellationToken = default)
    {
        var ids = new[] { firstId, secondId };
        return _chatDatabase
               .Chatroom
               .OfType<PrivateChatroom>()
               .Include(pc => pc.UserTickets)
               .Where(pc => pc.UserTickets.All(t => ids.Any(i => i == t.UserId)))
               .FirstOrDefaultAsync(cancellationToken);
    }
}