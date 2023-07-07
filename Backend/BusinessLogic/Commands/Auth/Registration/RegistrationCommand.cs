using MediatR;

namespace BusinessLogic.Commands.Auth.Registration;

public class RegistrationCommand : IRequest<RegistrationResponse>
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
}