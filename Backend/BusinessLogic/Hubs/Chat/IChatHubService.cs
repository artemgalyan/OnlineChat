using BusinessLogic.Models;
using Entities.Chatrooms;

namespace BusinessLogic.Hubs.Chat;

public interface IChatHubService
{
    public Task Send(Guid chatId, string message, CancellationToken cancellationToken);
    public Task NotifyUserAdded(Guid chatId, string username, CancellationToken cancellationToken);
    public Task NotifyUserKicked(Guid chatId, string username, CancellationToken cancellationToken);
    public Task NotifyUserLeft(Guid chatId, string username, CancellationToken cancellationToken);
    public Task AddChatroom(Chatroom chatroom, CancellationToken cancellationToken);
}