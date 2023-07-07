using Database;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BusinessLogic.Repository;

public interface IMessageRepository : IRepository<Message, Guid>
{
    public Task<List<Message>> GetByUserId(Guid userId, Guid chatId, CancellationToken cancellationToken);
}

public class MessageRepository : IMessageRepository
{
    private readonly ChatDatabase _chatDatabase;

    public MessageRepository(ChatDatabase chatDatabase)
    {
        _chatDatabase = chatDatabase;
    }

    public Task<List<Message>> GetAllAsync(CancellationToken cancellationToken)
    {
        return _chatDatabase.Messages.ToListAsync(cancellationToken);
    }

    public Task<Message?> GetByIdAsync(Guid key, CancellationToken cancellationToken)
    {
        return _chatDatabase.Messages
                            .FindAsync(new object?[] { key }, cancellationToken: cancellationToken)
                            .AsTask();
    }

    public Task<EntityEntry<Message>> InsertAsync(Message entity, CancellationToken cancellationToken)
    {
        return _chatDatabase.Messages.AddAsync(entity, cancellationToken).AsTask();
    }

    public Task<int> DeleteAsync(Guid entityKey, CancellationToken cancellationToken)
    {
        return _chatDatabase.Messages
                            .Where(m => m.Id == entityKey)
                            .ExecuteDeleteAsync(cancellationToken);
    }

    public Task UpdateAsync(Message entity, CancellationToken cancellationToken)
    {
        _chatDatabase.Messages.Update(entity);
        return Task.CompletedTask;
    }

    public Task<List<Message>> GetByUserId(Guid userId, Guid chatId, CancellationToken cancellationToken)
    {
        return _chatDatabase.Messages
                            .Where(m => m.SenderId == userId && m.ChatId == chatId)
                            .ToListAsync(cancellationToken);
    }
}