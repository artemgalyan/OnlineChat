using BusinessLogic.Hubs.Chat;
using BusinessLogic.Services.UsersService;
using Database;
using Entities;
using Entities.Chatrooms;
using Extensions;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Commands.Chatrooms.AddUserToChatroom;

public class AddUserToChatroomHandler : IRequestHandler<AddUserToChatroomCommand, AddUserToChatroomResponse>
{
    private readonly IStorageService _storageService;
    private readonly IUserAccessor _userAccessor;
    private readonly IChatHubService _chatHubService;

    public AddUserToChatroomHandler(IStorageService storageService, IUserAccessor userAccessor,
        IChatHubService chatHubService)
    {
        _storageService = storageService;
        _userAccessor = userAccessor;
        _chatHubService = chatHubService;
    }

    public async Task<AddUserToChatroomResponse> Handle(AddUserToChatroomCommand request,
        CancellationToken cancellationToken)
    {
        var tickets = await _storageService.GetChatroomTickets()
                                           .Where(t => t.ChatroomId == request.ChatId)
                                           .Include(t => t.User)
                                           .Include(t => t.Chatroom)
                                           .ToListAsync(cancellationToken);
        if (tickets.Count == 0)
        {
            return AddUserToChatroomResponse.ChatroomDoesntExist;
        }

        var chatroom = tickets.First().Chatroom;
        if (chatroom.Type == ChatType.Private)
        {
            return AddUserToChatroomResponse.ChatIsPrivate;
        }

        var currentUsername = _userAccessor.GetUsername()!;
        if (!tickets.Contains(u => u.User.Username == currentUsername))
        {
            return AddUserToChatroomResponse.AccessDenied;
        }

        if (tickets.Contains(t => t.User.Username == request.Username))
        {
            return AddUserToChatroomResponse.UserIsAlreadyInTheChat;
        }

        var user = await _storageService.GetUsers()
                                        .Where(u => u.Username == request.Username)
                                        .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return AddUserToChatroomResponse.UserDoesntExist;
        }
        var ticket = new ChatroomTicket(user, chatroom);

        var notifyTask = _chatHubService.NotifyUserAdded(chatId: chatroom.Id, currentUsername, cancellationToken);

        var adding = _storageService.AddChatroomTicketAsync(ticket, cancellationToken);
        await Task.WhenAll(notifyTask, adding);
        return AddUserToChatroomResponse.Success;
    }
}