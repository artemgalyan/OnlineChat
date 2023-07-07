using BusinessLogic.Services;
using BusinessLogic.Services.UsersService;
using Constants;
using Database;
using Entities;
using Entities.Chatrooms;
using Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Hubs.Chat;

/// <summary>
/// RPC Chatroom hub
/// </summary>
[Authorize(AuthenticationSchemes = Schemes.DefaultCookieScheme)]
public class ChatHub : Hub<IChatClientInterface>
{
    private readonly IStorageService _storageService;
    private readonly IUserAccessor _userAccessor;
    private readonly IUserConnectionTracker _tracker;
    private readonly IChatHubService _chatHubService;

    public ChatHub(IStorageService storageService, IUserAccessor userAccessor, IUserConnectionTracker tracker,
        IChatHubService chatHubService)
    {
        _storageService = storageService;
        _userAccessor = userAccessor;
        _tracker = tracker;
        _chatHubService = chatHubService;
    }

    [Authorize(AuthenticationSchemes = Schemes.DefaultCookieScheme)]
    public async Task<ConnectionResponseCode> Connect(string chatroomId)
    {
        if (!Guid.TryParse(chatroomId, out var chatId))
        {
            return ConnectionResponseCode.Error;
        }
        var id = _userAccessor.GetId()!;
        var username = _userAccessor.GetUsername()!;
        var ticket = await _storageService.GetChatroomTickets()
                                          .Where(t => t.UserId == id && t.ChatroomId == chatId)
                                          .Include(ticket => ticket.Chatroom)
                                          .FirstOrDefaultAsync(CancellationToken.None);

        if (ticket is null)
        {
            return ConnectionResponseCode.AccessDenied;
        }

        ticket.LastMessageRead = ticket.Chatroom.MessagesCount;
        var saving = _storageService.SaveChangesAsync(CancellationToken.None);
        var adding = Groups.AddToGroupAsync(connectionId: Context.ConnectionId,
            groupName: chatId.ToString());
        await Task.WhenAll(saving, adding);
        _tracker.SetChatId(username, chatId);
        return ConnectionResponseCode.SuccessfullyConnected;
    }

    [Authorize(AuthenticationSchemes = Schemes.DefaultCookieScheme)]
    public Task Disconnect(string chatId)
    {
        var username = _userAccessor.GetUsername()!;
        _tracker.SetChatId(username, null);
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
    }

    /// <summary>
    /// Method for users to send messages
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="message"></param>
    [Authorize(AuthenticationSchemes = Schemes.DefaultCookieScheme)]
    public Task Send(string chatId, string message)
    {
        return !Guid.TryParse(chatId, out var id)
            ? Task.CompletedTask
            : _chatHubService.SendAsync(id, message, CancellationToken.None);
    }

    [Authorize(AuthenticationSchemes = Schemes.DefaultCookieScheme)]
    public override async Task OnConnectedAsync()
    {
        _tracker.Add(username: _userAccessor.GetUsername()!, connectionId: Context.ConnectionId, chatId: null);
        await base.OnConnectedAsync();
    }

    [Authorize(AuthenticationSchemes = Schemes.DefaultCookieScheme)]
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _tracker.Remove(_userAccessor.GetUsername()!);
        await base.OnDisconnectedAsync(exception);
    }
}

public enum ConnectionResponseCode
{
    SuccessfullyConnected = 0,
    AccessDenied,
    Error,
    RoomDoesntExist
}