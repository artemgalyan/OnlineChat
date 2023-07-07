using MediatR;

namespace BusinessLogic.Commands.Auth.Login;

public class LoginCommand : IRequest<LoginResponse>
{
    public string Login { get; set; }
    public string Password { get; set; }
}