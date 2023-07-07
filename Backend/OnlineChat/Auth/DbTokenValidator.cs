using System.Net;
using BusinessLogic.Repository;

namespace OnlineChat.Auth;

public class DbTokenValidator : IMiddleware
{
    private readonly IUserTokenRepository _tokenRepository;

    public DbTokenValidator(IUserTokenRepository tokenRepository)
    {
        _tokenRepository = tokenRepository;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var headers = context.Request.Headers["Authentication"].FirstOrDefault()?.Split(" ");
        if (headers is null || headers.Length <= 1)
        {
            context.Response.Clear();
            context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        var token = headers[1];
        bool tokenIsValid = await _tokenRepository.GetByIdAsync(token) is not null;
        if (tokenIsValid)
        {
            await next(context);
            return;
        }
        context.Response.Clear();
        context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
        await context.Response.WriteAsync("Unauthorized");
    }
}