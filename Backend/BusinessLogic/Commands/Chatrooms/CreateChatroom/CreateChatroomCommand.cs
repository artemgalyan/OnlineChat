using Entities.Chatrooms;
using MediatR;

namespace BusinessLogic.Commands.Chatrooms.CreateChatroom;

public class CreateChatroomCommand : IRequest<CreateChatroomResponse>
{
    public ChatType Type { get; set; }
    public List<Guid> UserIds { get; set; }
    public string? Name { get; set; }
}