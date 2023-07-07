using MediatR;

namespace BusinessLogic.Commands.Chatrooms.AddUserToChatroom;

public class AddUserToChatroomCommand : IRequest<AddUserToChatroomResponse>
{
    public Guid UserId { get; set; }
    public Guid ChatId { get; set; }
}