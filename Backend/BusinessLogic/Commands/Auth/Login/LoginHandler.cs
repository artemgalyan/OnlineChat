using System.Security.Claims;
using BusinessLogic.Repository;
using BusinessLogic.Services;
using Constants;
using Entities;
using Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Commands.Auth.Login;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly IJwtProducer _jwtProducer;

    public LoginHandler(IUserRepository userRepository, IPasswordHasher<User> passwordHasher,
        IHttpContextAccessor accessor, IUserTokenRepository userTokenRepository, IJwtProducer jwtProducer)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _userTokenRepository = userTokenRepository;
        _jwtProducer = jwtProducer;
    }

    public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByLoginAsync(command.Login, cancellationToken);
        if (user is null)
        {
            return new LoginResponse { AuthToken = "", UserId = Guid.Empty };
        }

        var verifyResult = _passwordHasher.VerifyHashedPassword(user,
            providedPassword: command.Password,
            hashedPassword: user.Password
        );

        switch (verifyResult)
        {
            case PasswordVerificationResult.Failed:
                return new LoginResponse { AuthToken = "", UserId = Guid.Empty };
            case PasswordVerificationResult.SuccessRehashNeeded:
                user.Password = _passwordHasher.HashPassword(user, command.Password);
                await _userRepository.UpdateAsync(user, cancellationToken);
                break;
        }

        var claims = List.Of(
            new Claim(Claims.UserId, user.Id.ToString()),
            new Claim(Claims.Role, user.Role)
        );
        var token = _jwtProducer.MakeToken(claims);
        await _userTokenRepository.InsertAsync(new UserToken { JwtToken = token, UserId = user.Id }, cancellationToken);
        return new LoginResponse { AuthToken = token, UserId = user.Id };
    }
}