namespace Entities;

public class UserToken
{
    public Guid UserId { get; set; }
    public string JwtToken { get; set; }
}