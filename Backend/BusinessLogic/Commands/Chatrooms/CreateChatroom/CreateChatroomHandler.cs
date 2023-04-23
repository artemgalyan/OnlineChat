using BusinessLogic.Hubs.Chat;
using BusinessLogic.Services.UsersService;
using Database;
using Entities;
using Entities.Chatrooms;
using Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Commands.Chatrooms.CreateChatroom;

public class CreateChatroomHandler : IRequestHandler<CreateChatroomCommand, CreateChatroomResponse>
{
    private readonly IStorageService _storageService;
    private readonly IUserAccessor _userAccessor;
    private readonly IChatHubService _chatHubService;

    public CreateChatroomHandler(IStorageService storageService,
        IUserAccessor userAccessor, IChatHubService chatHubService)
    {
        _storageService = storageService;
        _userAccessor = userAccessor;
        _chatHubService = chatHubService;
    }

    public async Task<CreateChatroomResponse> Handle(CreateChatroomCommand request, CancellationToken cancellationToken)
    {
        if (request.Usernames.Count != 2 && request.Type != ChatType.Public || request.Usernames.Count == 0)
        {
            return CreateChatroomResponse.Failed;
        }
        if (request is { Type: ChatType.Public, Name: null })
        {
            return CreateChatroomResponse.Failed;
        }

        if (request.Usernames.Distinct().Count() != request.Usernames.Count)
        {
            return CreateChatroomResponse.Failed;
        }
        var username = _userAccessor.GetUsername()!;
        if (!request.Usernames.Contains(username))
        {
            return CreateChatroomResponse.Failed;
        }

        var users = await _storageService.GetUsers()
                                         .Where(u => request.Usernames.Any(n => n == u.Username))
                                         .Include(u => u.ChatroomTickets)
                                         .ThenInclude(t => t.Chatroom)
                                         .ToListAsync(cancellationToken);

        if (users.Count == 2 &&
            request.Type == ChatType.Private &&
            IsDuplicatePrivateChatroom(users[0], users[1]))
        {
            return CreateChatroomResponse.Failed;
        }

        var creator = users.First(u => u.Username == username);
        var id = Guid.NewGuid();
        Chatroom chatroom = request.Type == ChatType.Public
            ? new PublicChatroom(id, users, owner: creator, name: request.Name!)
            : new PrivateChatroom(id, users);
        
        await _storageService.AddChatroomAsync(chatroom, cancellationToken);
        await _storageService.SaveChangesAsync(cancellationToken);
        await _chatHubService.AddChatroom(chatroom, cancellationToken);
        return new CreateChatroomResponse(chatroom.Id);
    }

    private static bool IsDuplicatePrivateChatroom(User first, User second)
    {
        return first.Chatrooms.OfType<PrivateChatroom>()
                    .Any(c => c.Users.Contains(second));
    }
}