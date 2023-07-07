using BusinessLogic.Services.UsersService;
using Database;
using Entities.Chatrooms;
using Entities.Chatrooms.PublicChatroom;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Commands.Chatrooms.SetUsersRole;

public class SetUsersRoleHandler : IRequestHandler<SetUsersRoleRequest, SetUsersRoleResponse>
{
    private readonly IStorageService _storageService;
    private readonly IUserAccessor _userAccessor;

    public SetUsersRoleHandler(IStorageService storageService, IUserAccessor userAccessor)
    {
        _storageService = storageService;
        _userAccessor = userAccessor;
    }

    public async Task<SetUsersRoleResponse> Handle(SetUsersRoleRequest request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.GetId()!;
        var ticket = await _storageService.GetChatroomTickets()
                                          .Where(t => t.UserId == userId && t.ChatroomId == request.ChatId)
                                          .Include(t => t.Chatroom)
                                          .ThenInclude(c => c.Users)
                                          .FirstOrDefaultAsync(cancellationToken);

        if (ticket is null)
        {
            return SetUsersRoleResponse.BadRequest;
        }

        var chatroom = ticket.Chatroom;

        if (chatroom.Type == ChatType.Private)
        {
            return SetUsersRoleResponse.BadRequest;
        }

        var chat = (chatroom as PublicChatroom)!;
        if (chat.Administrators.Owner.Id != userId)
        {
            return SetUsersRoleResponse.AccessDenied;
        }

        var toSetRole = chat.Users.FirstOrDefault(u => u.Username == request.Username);
        if (toSetRole is null)
        {
            return SetUsersRoleResponse.UserIsNotInTheChat;
        }

        if (request.NewRole != UsersRole.Moderator)
        {
            chat.RemoveModerator(toSetRole);
        }

        return SetUsersRoleResponse.Success;
    }
}