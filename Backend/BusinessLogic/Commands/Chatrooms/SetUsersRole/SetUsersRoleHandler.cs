﻿using BusinessLogic.Services.UsersService;
using Database;
using Entities.Chatrooms;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Commands.Chatrooms.SetUsersRole;

public class SetUsersRoleHandler : IRequestHandler<SetUsersRoleRequest, SetUsersRoleResponse>
{
    private readonly IUsersService _usersService;

    public SetUsersRoleHandler(IStorageService storageService, IUsersService usersService,
        IHttpContextAccessor accessor)
    {
        _usersService = usersService;
    }

    public async Task<SetUsersRoleResponse> Handle(SetUsersRoleRequest request, CancellationToken cancellationToken)
    {
        var user = await _usersService.GetCurrentUser(cancellationToken);

        var chatroom = user?.Chatrooms.FirstOrDefault(c => c.Id == request.ChatId);
        if (chatroom is null)
        {
            return SetUsersRoleResponse.BadRequest;
        }

        if (chatroom.Type == ChatType.Private)
        {
            return SetUsersRoleResponse.BadRequest;
        }

        var chat = (chatroom as PublicChatroom)!;
        if (chat.Administrators.Owner != user!)
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