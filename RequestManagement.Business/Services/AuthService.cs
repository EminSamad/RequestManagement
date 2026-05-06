using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RequestManagement.Business.Interfaces;
using RequestManagement.Core.DTOs.Auth;
using RequestManagement.Core.DTOs.User;
using RequestManagement.Core.Entities;
using RequestManagement.Data.Repositories.Interfaces;

namespace RequestManagement.Business.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task RegisterAsync(RegisterDto dto)
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        if (users.Any(u => u.Email == dto.Email))
            throw new Exception("Email already exists");

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 0
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = 2,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = user.Id
        };
        
        await _unitOfWork.UserRoles.AddAsync(userRole);
        await _unitOfWork.SaveChangesAsync();


        await _emailService.SendEmailAsync(
            dto.Email,
            "Welcome to Request Management!",
            $"<h3>Welcome {dto.FullName}!</h3><p>Your account has been created successfully.</p>"
        );
    }

    public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _unitOfWork.GetUserWithRolesAsync(dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Invalid email or password");

        return await GenerateToken(user);
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var tokens = await _unitOfWork.RefreshTokens.GetAllAsync();
        var token = tokens.FirstOrDefault(t => t.Token == refreshToken
                                            && !t.IsRevoked
                                            && t.ExpiresAt > DateTime.UtcNow);

        if (token == null)
            throw new Exception("Invalid or expired refresh token");

        token.IsRevoked = true;
        await _unitOfWork.RefreshTokens.UpdateAsync(token);

        var user = await _unitOfWork.GetUserWithRolesByIdAsync(token.UserId);
        return await GenerateToken(user!);
    }

    private async Task<TokenResponseDto> GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddDays(1);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName)
        };

        if (user.UserRoles != null)
        {
            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
            }
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = user.Id
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return new TokenResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expires,
            RefreshToken = refreshToken.Token
        };
    }
}