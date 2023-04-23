using System.Security.Claims;
using Constants;
using Database;
using Entities;
using Extensions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Commands.Auth.Login;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IStorageService _storageService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IHttpContextAccessor _accessor;

    public LoginHandler(IStorageService storageService, IPasswordHasher<User> passwordHasher,
        IHttpContextAccessor accessor)
    {
        _storageService = storageService;
        _passwordHasher = passwordHasher;
        _accessor = accessor;
    }

    public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await _storageService.GetUsers()
                                        .Where(u => u.Username == command.Username)
                                        .FirstOrDefaultAsync(cancellationToken)
                                        .ConfigureAwait(false);
        if (user is null)
        {
            return LoginResponse.WrongUsername;
        }

        var verifyResult = _passwordHasher.VerifyHashedPassword(user,
            providedPassword: command.Password,
            hashedPassword: user.Password);

        if (verifyResult == PasswordVerificationResult.Failed)
        {
            return LoginResponse.WrongPassword;
        }

        var claims = List.Of(
            new Claim(Claims.Name, user.Username),
            new Claim(Claims.Token, user.Token.ToString()),
            new Claim(Claims.UserId, user.Id.ToString())
        );
        var identity = new ClaimsIdentity(claims, authenticationType: Schemes.DefaultCookieScheme);
        var principal = new ClaimsPrincipal(identity);
        await _accessor.HttpContext!.SignInAsync(principal);
        return LoginResponse.Success;
    }
}