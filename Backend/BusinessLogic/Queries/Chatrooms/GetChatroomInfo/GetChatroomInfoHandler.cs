using BusinessLogic.Models;
using BusinessLogic.Services.UsersService;
using Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Queries.Chatrooms.GetChatroomInfo;

public class GetChatroomInfoHandler : IRequestHandler<GetChatroomInfoRequest, object?>
{
    private readonly IUserAccessor _userAccessor;
    private readonly IStorageService _storageService;

    public GetChatroomInfoHandler(IUserAccessor userAccessor, IStorageService storageService)
    {
        _userAccessor = userAccessor;
        _storageService = storageService;
    }

    public async Task<object?> Handle(GetChatroomInfoRequest request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.GetId()!;
        var chatroomTicket = await _storageService.GetChatroomTickets()
                                                  .Include(t => t.Chatroom)
                                                  .ThenInclude(c => c.Users)
                                                  .Include(c => c.Chatroom)
                                                  .ThenInclude(c => c.Messages.OrderByDescending(m => m.SendingTime).Take(1))
                                                  .Where(ticket =>
                                                      ticket.ChatroomId == request.ChatId && ticket.UserId == userId)
                                                  .FirstOrDefaultAsync(cancellationToken);
        return chatroomTicket is null
            ? null
            : ChatroomInfo.Of(chatroomTicket);
    }
}