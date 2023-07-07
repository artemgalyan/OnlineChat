using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogic.Services;

public interface IJwtProducer
{
    public string MakeToken(IEnumerable<Claim> claims);
}

public class JwtProducer : IJwtProducer
{
    private readonly TimeSpan _tokenExpirationTime;
    private readonly SecurityKey _securityKey;
    private readonly JwtSecurityTokenHandler _securityTokenHandler = new JwtSecurityTokenHandler();

    public JwtProducer(TimeSpan tokenExpirationTime, RSA rsaKey)
    {
        _tokenExpirationTime = tokenExpirationTime;
        _securityKey = new RsaSecurityKey(rsaKey);
    }

    public string MakeToken(IEnumerable<Claim> claims)
    {
        var jwt = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now + _tokenExpirationTime,
            signingCredentials: new SigningCredentials(_securityKey, SecurityAlgorithms.RsaSha256)
        );
        var token = _securityTokenHandler.WriteToken(jwt);
        return token;
    }
}