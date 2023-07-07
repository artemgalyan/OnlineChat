namespace BusinessLogic.Commands.Auth.Login;

public class LoginResponse
{
    public string AuthToken { get; set; }
    public Guid UserId { get; set; }
}