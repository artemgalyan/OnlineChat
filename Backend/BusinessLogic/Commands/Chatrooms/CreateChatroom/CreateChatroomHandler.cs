using BusinessLogic.Hubs.Chat;
using BusinessLogic.Repository;
using BusinessLogic.Services.UsersService;
using Entities;
using Entities.Chatrooms;
using Entities.Chatrooms.PublicChatroom;
using MediatR;

namespace BusinessLogic.Commands.Chatrooms.CreateChatroom;

public class CreateChatroomHandler : IRequestHandler<CreateChatroomCommand, CreateChatroomResponse>
{
    private readonly IUserAccessor _userAccessor;
    private readonly IChatHubService _chatHubService;
    private readonly IChatroomRepository _chatroomRepository;
    private readonly IUserRepository _userRepository;
    private readonly IChatroomTicketRepository _chatroomTicketRepository;

    public CreateChatroomHandler(IUserAccessor userAccessor, IChatHubService chatHubService,
        IChatroomRepository chatroomRepository, IUserRepository userRepository, IChatroomTicketRepository chatroomTicketRepository)
    {
        _userAccessor = userAccessor;
        _chatHubService = chatHubService;
        _chatroomRepository = chatroomRepository;
        _userRepository = userRepository;
        _chatroomTicketRepository = chatroomTicketRepository;
    }

    public async Task<CreateChatroomResponse> Handle(CreateChatroomCommand request, CancellationToken cancellationToken)
    {
        if (request.UserIds.Count != 2 && request.Type != ChatType.Public || request.UserIds.Count == 0)
        {
            return CreateChatroomResponse.Failed;
        }
        if (request is { Type: ChatType.Public, Name: null })
        {
            return CreateChatroomResponse.Failed;
        }

        if (await _userRepository.ContainsAllAsync(request.UserIds, cancellationToken) is false)
        {
            return CreateChatroomResponse.Failed;
        }
        if (request.UserIds.Distinct().Count() != request.UserIds.Count || request.UserIds.Contains(Guid.Empty))
        {
            return CreateChatroomResponse.Failed;
        }
        var userId = _userAccessor.GetId();
        if (!request.UserIds.Contains(userId))
        {
            return CreateChatroomResponse.Failed;
        }


        if (request.UserIds.Count == 2 &&
            request.Type == ChatType.Private &&
            await IsDuplicatePrivateChatroom(request.UserIds[0], request.UserIds[1], cancellationToken))
        {
            return CreateChatroomResponse.Failed;
        }

        var mappedUsers = request.UserIds
                                 .Select(i => new User { Id = i })
                                 .ToList();
        var owner = new User { Id = _userAccessor.GetId() };
        var id = Guid.NewGuid();
        Chatroom chatroom = request.Type == ChatType.Public
            ? new PublicChatroom(id, new List<User>(), owner: owner, name: request.Name!)
            : new PrivateChatroom(id, mappedUsers);

        await _chatroomRepository.InsertAsync(chatroom, cancellationToken);
        await _chatHubService.AddChatroomAsync(chatroom, cancellationToken);
        var tickets = request.UserIds.Select(i =>
            new ChatroomTicket { ChatroomId = chatroom.Id, LastMessageRead = 0, UserId = i });
        await _chatroomTicketRepository.InsertAllAsync(tickets, cancellationToken);
        return new CreateChatroomResponse(chatroom.Id);
    }

    private async Task<bool> IsDuplicatePrivateChatroom(Guid first, Guid second, CancellationToken cancellationToken)
    {
        return await _chatroomRepository.GetPrivateChatroomByUsersAsync(first, second, cancellationToken) is not null;
    }
}