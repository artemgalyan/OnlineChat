using Database;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BusinessLogic.Repository;

public interface IChatroomTicketRepository : IRepository<ChatroomTicket, (Guid UserId, Guid ChatId)>
{
    public Task<List<ChatroomTicket>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    public Task<List<ChatroomTicket>> GetAllByChatroomIdAsync(Guid chatId, CancellationToken cancellationToken = default);
    public Task InsertAllAsync(IEnumerable<ChatroomTicket> chatroomTickets,
        CancellationToken cancellationToken = default);
}

public class ChatroomTicketRepository : IChatroomTicketRepository
{
    private readonly ChatDatabase _chatDatabase;

    public ChatroomTicketRepository(ChatDatabase chatDatabase)
    {
        _chatDatabase = chatDatabase;
    }

    public Task<List<ChatroomTicket>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _chatDatabase.ChatroomTicket.ToListAsync(cancellationToken);
    }

    public Task<ChatroomTicket?> GetByIdAsync((Guid UserId, Guid ChatId) key,
        CancellationToken cancellationToken = default)
    {
        return _chatDatabase.ChatroomTicket
                            .FindAsync(new object?[] { key.UserId, key.ChatId },
                                cancellationToken: cancellationToken)
                            .AsTask();
    }

    public Task<EntityEntry<ChatroomTicket>> InsertAsync(ChatroomTicket entity,
        CancellationToken cancellationToken = default)
    {
        return _chatDatabase.ChatroomTicket.AddAsync(entity, cancellationToken)
                            .AsTask();
    }

    public Task<int> DeleteAsync((Guid UserId, Guid ChatId) entityKey, CancellationToken cancellationToken = default)
    {
        return _chatDatabase.ChatroomTicket
                            .Where(t => t.UserId == entityKey.UserId && t.ChatroomId == entityKey.ChatId)
                            .ExecuteDeleteAsync(cancellationToken);
    }

    public Task UpdateAsync(ChatroomTicket entity, CancellationToken cancellationToken = default)
    {
        _chatDatabase.ChatroomTicket.Update(entity);
        return Task.CompletedTask;
    }

    public Task<List<ChatroomTicket>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _chatDatabase.ChatroomTicket
                            .Where(t => t.UserId == userId)
                            .ToListAsync(cancellationToken);
    }

    public Task<List<ChatroomTicket>> GetAllByChatroomIdAsync(Guid chatId, CancellationToken cancellationToken = default)
    {
        return _chatDatabase.ChatroomTicket
                            .Where(t => t.ChatroomId == chatId)
                            .ToListAsync(cancellationToken);
    }

    public Task InsertAllAsync(IEnumerable<ChatroomTicket> chatroomTickets, CancellationToken cancellationToken = default)
    {
        return _chatDatabase.ChatroomTicket.AddRangeAsync(chatroomTickets, cancellationToken);
    }
}