using RequestManagement.Domain.DTOs.Auth;
using RequestManagement.Domain.DTOs.User;

namespace RequestManagement.Application.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDto> LoginAsync(LoginDto loginDto);
    Task RegisterAsync(RegisterDto registerDto);
    Task<TokenResponseDto> RefreshTokenAsync(string refreshToken);
    Task InviteUserAsync(string email, int roleId);
    Task RegisterWithTokenAsync(RegisterDto registerDto,string token);
}