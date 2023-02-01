﻿using BusinessLogic.Hubs.Chat;
using BusinessLogic.Services.UsersService;
using Database;
using Entities.Chatrooms;
using Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace BusinessLogic.Commands.Chatrooms.AddUserToChatroom;

public class AddUserToChatroomHandler : IRequestHandler<AddUserToChatroomCommand, AddUserToChatroomResponse>
{
    private readonly IStorageService _storageService;
    private readonly IUsersService _usersService;
    private readonly IHubContext<ChatHub> _hubContext;

    public AddUserToChatroomHandler(IStorageService storageService, IUsersService usersService,
        IHttpContextAccessor accessor, IHubContext<ChatHub> hubContext)
    {
        _storageService = storageService;
        _usersService = usersService;
        _hubContext = hubContext;
    }

    public async Task<AddUserToChatroomResponse> Handle(AddUserToChatroomCommand request,
        CancellationToken cancellationToken)
    {
        var chatroom = await _storageService.GetChatroomById(request.ChatId, cancellationToken);
        if (chatroom is null)
        {
            return AddUserToChatroomResponse.ChatroomDoesntExist;
        }

        if (chatroom.Type == ChatType.Private)
        {
            return AddUserToChatroomResponse.ChatIsPrivate;
        }

        var currentUsername = _usersService.GetUsername()!;
        var currentToken = _usersService.GetToken()!;
        if (!chatroom.Users.Contains(u => u.Username == currentUsername && u.Token == currentToken))
        {
            return AddUserToChatroomResponse.AccessDenied;
        }

        if (chatroom.Users.Contains(u => u.Username == request.Username))
        {
            return AddUserToChatroomResponse.UserIsAlreadyInTheChat;
        }

        var user = await _storageService.GetUserByUsername(request.Username, cancellationToken);
        if (user is null)
        {
            return AddUserToChatroomResponse.UserDoesntExist;
        }

        var notifyTask =
            _hubContext.NotifyUserAdded(chatId: chatroom.Id.ToString(), currentUsername, cancellationToken);
        user.Join(chatroom);
        var saveTask = _storageService.SaveChangesAsync(cancellationToken);
        await Task.WhenAll(notifyTask, saveTask);
        return AddUserToChatroomResponse.Success;
    }
}