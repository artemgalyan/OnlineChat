using BusinessLogic.Repository;
using Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Commands.Auth.Registration;

public class RegistrationRequestHandler : IRequestHandler<RegistrationCommand, RegistrationResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public RegistrationRequestHandler(IPasswordHasher<User> passwordHasher, IUserRepository userRepository)
    {
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
    }

    public async Task<RegistrationResponse> Handle(RegistrationCommand command, CancellationToken cancellationToken)
    {
        if (await _userRepository.GetByLoginAsync(command.Login, cancellationToken) is not null)
        {
            return RegistrationResponse.DuplicateLogin;
        }

        var user = new User 
        {
            Id = Guid.NewGuid(),
            Login = command.Login,
            Name = command.Name,
            Surname = command.Surname,
            Password = ""
        };
        var password = _passwordHasher.HashPassword(user, command.Password);
        user.Password = password;
        await _userRepository.InsertAsync(user, cancellationToken);
        return RegistrationResponse.Success;
    }
}