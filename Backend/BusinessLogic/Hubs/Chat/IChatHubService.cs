using BusinessLogic.Models;
using Entities;
using Entities.Chatrooms;

namespace BusinessLogic.Hubs.Chat;

public interface IChatHubService
{
    public Task SendAsync(Guid chatId, string message, CancellationToken cancellationToken);
    public Task NotifyUserAddedAsync(Guid chatId, User user, CancellationToken cancellationToken);
    public Task NotifyUserKickedAsync(Guid chatId, User user, CancellationToken cancellationToken);
    public Task NotifyUserLeftAsync(Guid chatId, User user, CancellationToken cancellationToken);
    public Task AddChatroomAsync(Chatroom chatroom, CancellationToken cancellationToken);
}