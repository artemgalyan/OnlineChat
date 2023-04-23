using MediatR;

namespace BusinessLogic.Commands.Chatrooms.SendMessage;

public class SendMessageRequest : IRequest<Unit>
{
    public Guid ChatId { get; set; }
    public string Text { get; set; }
}