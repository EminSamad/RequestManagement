namespace RequestManagement.Core.DTOs.Auth;
public class TokenResponseDto
{
    public string Token { get; set; } = null!;
    public DateTime Expiration { get; set; }
}