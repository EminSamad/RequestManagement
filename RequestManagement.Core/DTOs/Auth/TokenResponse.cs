namespace RequestManagement.Core.DTOs.Auth;
public class TokenResponse
{
    public string Token { get; set; } = null!;
    public DateTime Expiration { get; set; }
}