using BusinessLogic.Models;
using BusinessLogic.Services;
using BusinessLogic.Services.UsersService;
using Database;
using Entities;
using Entities.Chatrooms;
using Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Hubs.Chat;

public class ChatHubService : IChatHubService
{
    private readonly IStorageService _storageService;
    private readonly IUserAccessor _userAccessor;
    private readonly IUserConnectionTracker _tracker;
    private readonly IHubContext<ChatHub, IChatClientInterface> _hubContext;

    public ChatHubService(IStorageService storageService, IUserAccessor userAccessor,
        IUserConnectionTracker tracker, IHubContext<ChatHub, IChatClientInterface> hubContext)
    {
        _storageService = storageService;
        _userAccessor = userAccessor;
        _tracker = tracker;
        _hubContext = hubContext;
    }

    /// <summary>
    /// Method for users to send messages
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    public async Task Send(Guid chatId, string message, CancellationToken cancellationToken)
    {
        var username = _userAccessor.GetUsername()!;
        var chatroom = await _storageService.GetChatrooms()
                                            .Where(c => c.Id == chatId)
                                            .Include(c => c.Messages.Take(0))
                                            .Include(c => c.UserTickets)
                                            .ThenInclude(t => t.User)
                                            .FirstOrDefaultAsync(cancellationToken);
        if (chatroom is null || !chatroom.Users.Contains(u => u.Username == username))
        {
            return;
        }

        var messageObject = new Message(username, message);
        await SendMessageToChat(chatroom, messageObject, cancellationToken);
    }

    public Task NotifyUserAdded(Guid chatId, string username, CancellationToken cancellationToken)
    {
        return NotifyAll(chatId, $"User {username} was kicked from the chat", cancellationToken);
    }

    public Task NotifyUserKicked(Guid chatId, string username, CancellationToken cancellationToken)
    {
        return NotifyAll(chatId, $"User {username} was kicked from the chat", cancellationToken);
    }

    public Task NotifyUserLeft(Guid chatId, string username, CancellationToken cancellationToken)
    {
        return NotifyAll(chatId, $"User ${username} left the chat", cancellationToken);
    }

    public Task AddChatroom(Chatroom chatroom, CancellationToken cancellationToken)
    {
        var users = chatroom.Users
                            .Select(u => _tracker.GetInfo(u.Username))
                            .Where(t => t.HasValue)
                            .Select(t => t!.Value.ConnectionId)
                            .ToList();
        return _hubContext.Clients.Clients(users).AddChatroom(ChatroomInfo.OfNew(chatroom));
    }

    /// <summary>
    /// Method for sending message to chat
    /// Warning: method doesn't check if the caller is in chat  
    /// </summary>
    /// <param name="chatroom"></param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    private async Task SendMessageToChat(Chatroom chatroom, Message message, CancellationToken cancellationToken)
    {
        chatroom.AddMessage(message);
        var count = chatroom.MessagesCount;
        
        var usersOnlineInChat = chatroom.Users
                                        .Where(u =>
                                        {
                                            var info = _tracker.GetInfo(u.Username);
                                            if (!info.HasValue)
                                            {
                                                return false;
                                            }
                                            return info.Value.ChatId == chatroom.Id;
                                        })
                                        .Select(u => u.Id)
                                        .ToList();
        var updating = _storageService.GetChatroomTickets()
                                      .Where(id =>
                                          id.ChatroomId == chatroom.Id && usersOnlineInChat.Any(u => u == id.UserId))
                                      .ForEachAsync(t => t.LastMessageRead = count, CancellationToken.None);
        var sending = _hubContext.Clients
                                 .Group(chatroom.Id.ToString())
                                 .Receive(message);
        var promoting = PromoteChatToTop(chatroom, message);
        await Task.WhenAll(sending, promoting, updating);
        await _storageService.SaveChangesAsync(cancellationToken);
    }

    private async Task PromoteChatToTop(Chatroom chatroom, Message latestMessage)
    {
        List<string> connectionIds = chatroom.Users
                                             .Select(u => u.Username)
                                             .Select(n => _tracker.GetInfo(n))
                                             .Where(t => t.HasValue)
                                             .Select(t => t!.Value.ConnectionId)
                                             .ToList();

        await _hubContext.Clients.Clients(connectionIds).PromoteToTop(chatroom.Id.ToString(), latestMessage);
    }

    private Task NotifyAll(Guid chatId, string message, CancellationToken cancellationToken)
    {
        return _hubContext.Clients.Group(chatId.ToString()).Receive(new Message("", message));
    }
}