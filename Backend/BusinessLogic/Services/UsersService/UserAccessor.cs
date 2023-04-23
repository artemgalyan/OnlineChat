using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Services.UsersService;

public class UserAccessor : IUserAccessor
{
    private readonly ClaimsPrincipal _claimsPrincipal;

    public UserAccessor(IHttpContextAccessor accessor)
    {
        _claimsPrincipal = accessor.HttpContext!.User;
    }
    public bool TryGetClaim(string claimName, out string result)
    {
        var claims = _claimsPrincipal.Claims;
        var claim = claims.FirstOrDefault(c => c.Type == claimName);
        result = claim?.Value!;
        return claim is not null;
    }
}