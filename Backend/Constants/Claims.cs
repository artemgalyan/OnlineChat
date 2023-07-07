using System.Security.Claims;

namespace Constants;

public static class Claims
{
    public const string Name = ClaimTypes.Name;
    public const string UserId = "UserId";
    public const string Token = "Token";
    public const string Role = ClaimsIdentity.DefaultRoleClaimType;
}