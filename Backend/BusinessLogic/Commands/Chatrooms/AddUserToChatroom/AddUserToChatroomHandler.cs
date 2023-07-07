using BusinessLogic.Hubs.Chat;
using BusinessLogic.Repository;
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
    private readonly IChatroomTicketRepository _chatroomTicketRepository;
    private readonly IChatroomRepository _chatroomRepository;
    private readonly IChatHubService _chatHubService;
    private readonly IUserRepository _userRepository;

    public AddUserToChatroomHandler(IUserAccessor userAccessor,
        IChatHubService chatHubService, IChatroomTicketRepository chatroomTicketRepository,
        IChatroomRepository chatroomRepository, IUserRepository userRepository)
    {
        _chatHubService = chatHubService;
        _chatroomTicketRepository = chatroomTicketRepository;
        _chatroomRepository = chatroomRepository;
        _userRepository = userRepository;
    }

    public async Task<AddUserToChatroomResponse> Handle(AddUserToChatroomCommand request,
        CancellationToken cancellationToken)
    {
        var chatroom = await _chatroomRepository.GetByIdAsync(request.ChatId, cancellationToken);
        if (chatroom is null)
        {
            return AddUserToChatroomResponse.ChatroomDoesntExist;
        }

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (await _chatroomTicketRepository.GetByIdAsync((request.UserId, request.ChatId), cancellationToken) is not
            null)
        {
            return AddUserToChatroomResponse.UserIsAlreadyInTheChat;
        }

        if (chatroom is PrivateChatroom)
        {
            return AddUserToChatroomResponse.ChatIsPrivate;
        }

        var ticket = new ChatroomTicket 
        {
            UserId = request.UserId, 
            ChatroomId = request.ChatId, 
            LastMessageRead = chatroom.MessagesCount,
        };

        if (user is null)
        {
            return AddUserToChatroomResponse.UserDoesntExist;
        }

        var notifyTask = _chatHubService.NotifyUserAddedAsync(chatId: chatroom.Id, user, cancellationToken);
        var adding = _chatroomTicketRepository.InsertAsync(ticket, cancellationToken);
        await Task.WhenAll(notifyTask, adding);
        return AddUserToChatroomResponse.Success;
    }
}