using RequestManagement.Core.DTOs.Auth;
using RequestManagement.Core.DTOs.User;

namespace RequestManagement.Business.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDto> LoginAsync(LoginDto loginDto);
    Task RegisterAsync(RegisterDto registerDto);
    Task<TokenResponseDto> RefreshTokenAsync(string refreshToken);
    Task InviteUserAsync(string email, int roleId);
    Task RegisterWithTokenAsync(RegisterDto registerDto,string token);
}