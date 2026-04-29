namespace RequestManagement.Core.DTOs.Auth;
public class TokenResponseDto
{
    public string Token { get; set; } = null!;
    public DateTime Expiration { get; set; }
    public string RefreshToken { get; set; } = null!;
}