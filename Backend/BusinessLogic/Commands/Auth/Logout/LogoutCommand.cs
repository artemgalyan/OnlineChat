using MediatR;

namespace BusinessLogic.Commands.Auth.Logout;

public struct LogoutCommand : IRequest<Unit>
{
    public string Token { get; set; }
}