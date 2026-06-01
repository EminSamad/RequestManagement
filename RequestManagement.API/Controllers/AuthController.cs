using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RequestManagement.Application.Interfaces;
using RequestManagement.Core.DTOs.Auth;
using RequestManagement.Core.DTOs.User;
using RequestManagement.Core.Entities;

namespace RequestManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        _logger.LogInformation("New user registration attempt: {Email}", dto.Email);
        await _authService.RegisterAsync(dto);
        _logger.LogInformation("User registered successfully: {Email}", dto.Email);
        return Ok("User registered successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        _logger.LogInformation("Login attempt: {Email}", dto.Email);
        var result = await _authService.LoginAsync(dto);
        _logger.LogInformation("User logged in successfully: {Email}", dto.Email);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        _logger.LogInformation("Refresh token request");
        var result = await _authService.RefreshTokenAsync(refreshToken);
        _logger.LogInformation("Token refreshed successfully");
        return Ok(result);
    }
    [HttpPost("invite")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Invite([FromBody] InviteDto dto)
    {
        _logger.LogInformation("Admin inviting user: {Email}", dto.Email);
        await _authService.InviteUserAsync(dto.Email, dto.RoleId);
        _logger.LogInformation("Invitation sent to: {Email}", dto.Email);
        return Ok("Invitation sent successfully");
    }

    [HttpPost("register-with-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterWithToken([FromBody] RegisterDto dto, [FromQuery] string token)
    {
        _logger.LogInformation("User registering with invite token: {Email}", dto.Email);
        await _authService.RegisterWithTokenAsync(dto, token);
        _logger.LogInformation("User registered with invite token: {Email}", dto.Email);
        return Ok("User registered successfully");
    }
}