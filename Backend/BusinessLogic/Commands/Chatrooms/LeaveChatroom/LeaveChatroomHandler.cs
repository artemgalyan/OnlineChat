using BusinessLogic.Hubs.Chat;
using BusinessLogic.Services.UsersService;
using Database;
using Entities.Chatrooms;
using Entities.Chatrooms.PublicChatroom;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Commands.Chatrooms.LeaveChatroom;

public class LeaveChatroomHandler : IRequestHandler<LeaveChatroomCommand, LeaveChatroomResponse>
{
    private readonly IUserAccessor _userAccessor;
    private readonly IStorageService _storageService;
    private readonly IChatHubService _chatHubService;

    public LeaveChatroomHandler(IUserAccessor userAccessor, IStorageService storageService, IChatHubService chatHubService)
    {
        _userAccessor = userAccessor;
        _storageService = storageService;
        _chatHubService = chatHubService;
    }

    public async Task<LeaveChatroomResponse> Handle(LeaveChatroomCommand request, CancellationToken cancellationToken)
    {
        var id = _userAccessor.GetId()!;
        var ticket = await _storageService.GetChatroomTickets()
                                          .Where(t => t.UserId == id && t.ChatroomId == request.ChatId)
                                          .Include(t => t.Chatroom)
                                          .Include(t => t.User)
                                          .FirstOrDefaultAsync(cancellationToken);

        if (ticket is null)
        {
            return LeaveChatroomResponse.BadRequest;
        }

        var chatroom = ticket.Chatroom;

        if (chatroom.Type == ChatType.Private)
        {
            return LeaveChatroomResponse.BadRequest;
        }

        var user = ticket.User;
        var chat = (chatroom as PublicChatroom)!;
        if (chat.Kick(user))
        {
            var savingTask = _storageService.SaveChangesAsync(cancellationToken);
            var sendingTask = _chatHubService.NotifyUserLeftAsync(chat.Id, user.Username, cancellationToken);
            await Task.WhenAll(savingTask, sendingTask);
            return LeaveChatroomResponse.Success;
        }

        if (chat.Administrators.Owner != user)
        {
            return LeaveChatroomResponse.BadRequest;
        }

        if (chat.UsersCount == 1)
        {
            user.Leave(chat);
            await _storageService.RemoveChatroomAsync(chat, cancellationToken);
            await _storageService.SaveChangesAsync(cancellationToken);
            return LeaveChatroomResponse.Success;
        }

        var newOwner = chat.Administrators.Moderators.FirstOrDefault() ?? chat.Users.First();

        chat.ForceKick(user);
        chat.Administrators.Owner = newOwner;
        var saving = _storageService.SaveChangesAsync(cancellationToken);
        var sending = _chatHubService.NotifyUserLeftAsync(chat.Id, user.Username, cancellationToken);
        await Task.WhenAll(saving, sending);
        return LeaveChatroomResponse.Success;
    }
}