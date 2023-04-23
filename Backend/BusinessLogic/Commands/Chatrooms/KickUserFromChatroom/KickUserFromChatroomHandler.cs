using BusinessLogic.Hubs.Chat;
using Database;
using Entities;
using Entities.Chatrooms;
using Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Commands.Chatrooms.KickUserFromChatroom;

public class KickUserFromChatroomHandler : IRequestHandler<KickUserFromChatroomCommand, KickUserFromChatroomResponse>
{
    private readonly IStorageService _storageService;
    private readonly IChatHubService _chatHubService;

    public KickUserFromChatroomHandler(IStorageService storageService, IChatHubService chatHubService)
    {
        _storageService = storageService;
        _chatHubService = chatHubService;
    }

    public async Task<KickUserFromChatroomResponse> Handle(KickUserFromChatroomCommand request,
        CancellationToken cancellationToken)
    {
        var ticket = await _storageService.GetChatroomTickets()
                                          .Include(t => t.User)
                                          .Where(t => t.User.Username == request.Username &&
                                                      t.ChatroomId == request.ChatId)
                                          .Include(t => t.Chatroom)
                                          .FirstOrDefaultAsync(cancellationToken);

        if (ticket is null)
        {
            return KickUserFromChatroomResponse.BadRequest;
        }
        
        var chatroom = ticket.Chatroom;
        var user = ticket.User;

        if (!chatroom.Users.Contains(user))
        {
            return KickUserFromChatroomResponse.BadRequest;
        }

        var toKick = chatroom.Users.FirstOrDefault(u => u.Username == request.Username);
        if (toKick is null)
        {
            return KickUserFromChatroomResponse.UserIsNotInChatroom;
        }

        var chat = (chatroom as PublicChatroom)!;
        if (toKick.IsOwnerOf(chat))
        {
            return KickUserFromChatroomResponse.AccessDenied;
        }
        if (user.IsOwnerOf(chat) || user.IsModeratorOf(chat) && !toKick.IsModeratorOf(chat))
        {
            return await Kick(chat, toKick, cancellationToken);
        }
        return KickUserFromChatroomResponse.AccessDenied;
    }

    private async Task<KickUserFromChatroomResponse> Kick(PublicChatroom chatroom, User user,
        CancellationToken cancellationToken)
    {
        chatroom.Kick(user);
        var saving = _storageService.SaveChangesAsync(cancellationToken);
        var notifying = _chatHubService.NotifyUserKicked(chatroom.Id, user.Username, cancellationToken);
        await Task.WhenAll(saving, notifying);
        return KickUserFromChatroomResponse.Success;
    }
}