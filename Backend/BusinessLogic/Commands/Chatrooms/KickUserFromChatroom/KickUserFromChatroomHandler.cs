using BusinessLogic.Hubs.Chat;
using BusinessLogic.Repository;
using BusinessLogic.Services.UsersService;
using Entities.Chatrooms;
using Entities.Chatrooms.PublicChatroom;
using MediatR;

namespace BusinessLogic.Commands.Chatrooms.KickUserFromChatroom;

public class KickUserFromChatroomHandler : IRequestHandler<KickUserFromChatroomCommand, KickUserFromChatroomResponse>
{
    private const int UserRole = 0;
    private const int ModeratorRole = 1;
    private const int OwnerRole = 2;
    private readonly IUserAccessor _userAccessor;
    private readonly IChatroomTicketRepository _chatroomTicketRepository;
    private readonly IChatHubService _chatHubService;
    private readonly IAdministratorsRepository _administratorsRepository;
    private readonly IChatroomRepository _chatroomRepository;
    private readonly IUserAdministratorRepository _userAdministratorRepository;
    private readonly IUserRepository _userRepository;

    public KickUserFromChatroomHandler(IChatHubService chatHubService,
        IChatroomTicketRepository chatroomTicketRepository, IAdministratorsRepository administratorsRepository, IChatroomRepository chatroomRepository, IUserAdministratorRepository userAdministratorRepository, IUserAccessor userAccessor, IUserRepository userRepository)
    {
        _chatHubService = chatHubService;
        _chatroomTicketRepository = chatroomTicketRepository;
        _administratorsRepository = administratorsRepository;
        _chatroomRepository = chatroomRepository;
        _userAdministratorRepository = userAdministratorRepository;
        _userAccessor = userAccessor;
        _userRepository = userRepository;
    }

    public async Task<KickUserFromChatroomResponse> Handle(KickUserFromChatroomCommand request,
        CancellationToken cancellationToken)
    {
        if (request.UserId == _userAccessor.GetId())
        {
            return KickUserFromChatroomResponse.AccessDenied;
        }
        var ticket = await _chatroomTicketRepository.GetByIdAsync((request.UserId, request.ChatId), cancellationToken);
        if (ticket is null)
        {
            return KickUserFromChatroomResponse.BadRequest;
        }

        if (await _chatroomRepository.GetByIdAsync(request.ChatId, cancellationToken) is PrivateChatroom)
        {
            return KickUserFromChatroomResponse.AccessDenied;
        }

        var administrators = await _administratorsRepository.GetByChatroomId(request.ChatId, cancellationToken);
        var selfRole = await GetRoleAsync(_userAccessor.GetId(), request.ChatId, administrators!, cancellationToken);
        if (selfRole == 0)
        {
            return KickUserFromChatroomResponse.AccessDenied;
        }
        var otherRole = await GetRoleAsync(request.UserId, request.ChatId, administrators!, cancellationToken);
        if (otherRole >= selfRole)
        {
            return KickUserFromChatroomResponse.AccessDenied;
        }

        var kickedUser = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        var kicking = _chatroomTicketRepository.DeleteAsync((request.UserId, request.ChatId), cancellationToken);
        var notifying = _chatHubService.NotifyUserKickedAsync(request.ChatId, kickedUser!, cancellationToken);
        await Task.WhenAll(kicking, notifying);
        return KickUserFromChatroomResponse.Success;
    }

    private async Task<int> GetRoleAsync(Guid userId, Guid chatId, Administrators administrators, CancellationToken cancellationToken)
    {
        if (administrators.OwnerId == userId)
        {
            return OwnerRole;
        }
        if (await _userAdministratorRepository.GetByIdAsync((userId, administrators.Id), cancellationToken) is not null)
        {
            return ModeratorRole;
        }
        return UserRole;
    }
}