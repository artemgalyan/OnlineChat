using BusinessLogic.Models;
using BusinessLogic.Services.UsersService;
using Database;
using Entities;
using Entities.Chatrooms;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Queries.Chatrooms.GetChatrooms;

public class GetChatroomsHandler : IRequestHandler<GetChatroomsRequest, GetChatroomsResult>
{
    private readonly IUserAccessor _userAccessor;
    private readonly IStorageService _storageService;

    public GetChatroomsHandler(IUserAccessor userAccessor, IStorageService storageService)
    {
        _userAccessor = userAccessor;
        _storageService = storageService;
    }

    public async Task<GetChatroomsResult> Handle(GetChatroomsRequest request, CancellationToken cancellationToken)
    {
        var id = _userAccessor.GetId()!;
        var tickets = await _storageService.GetChatroomTickets()
                                           .Where(ticket => ticket.UserId == id)
                                           .Include(t => t.Chatroom)
                                           .ThenInclude(c => c.UserTickets)
                                           .ThenInclude(u => u.User)
                                           .Include(c => c.Chatroom)
                                           .ThenInclude(c => c.Messages.OrderByDescending(m => m.SendingTime).Take(1))
                                           .ToListAsync(cancellationToken);
        var result = tickets.Select(ChatroomInfo.Of).ToList();
        result.Sort(SortComparator);
        return new GetChatroomsResult(result);
    }

    private static int SortComparator(object a, object b)
    {
        if ((a as ChatroomInfoBase)!.LastMessage is null)
        {
            return 1;
        }
        if ((b as ChatroomInfoBase)!.LastMessage is null)
        {
            return -1;
        }
        return -(a as ChatroomInfoBase)!.LastMessage!.SendingTime.CompareTo((b as ChatroomInfoBase)!.LastMessage!.SendingTime);
    }
}