using BusinessLogic.Repository;
using MediatR;

namespace BusinessLogic.Commands.Auth.Logout;

public class LogoutHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IUserTokenRepository _userTokenRepository;

    public LogoutHandler(IUserTokenRepository userTokenRepository)
    {
        _userTokenRepository = userTokenRepository;
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _userTokenRepository.DeleteAsync(request.Token, cancellationToken);
        return default;
    }
}