using BusinessLogic.Hubs.Chat;
using MediatR;

namespace BusinessLogic.Commands.Chatrooms.SendMessage;

public class SendMessageHandler : IRequestHandler<SendMessageRequest, Unit>
{
    private readonly IChatHubService _chatHubService;

    public SendMessageHandler(IChatHubService chatHubService)
    {
        _chatHubService = chatHubService;
    }

    public async Task<Unit> Handle(SendMessageRequest request, CancellationToken cancellationToken)
    {
        await _chatHubService.Send(request.ChatId, request.Text, cancellationToken);
        return default;
    }
}