using MediatR;

namespace BusinessLogic.Commands.Chatrooms.KickUserFromChatroom;

public class KickUserFromChatroomCommand : IRequest<KickUserFromChatroomResponse>
{
    public Guid UserId { get; set; }
    public Guid ChatId { get; set; }
}