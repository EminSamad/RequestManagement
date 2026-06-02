using FastEndpoints;
using RequestManagement.Application.Interfaces;
using RequestManagement.Domain.DTOs.User;

namespace RequestManagement.API.Endpoints.Auth;

public class RegisterEndpoint : Endpoint<RegisterDto>
{
    private readonly IAuthService _authService;

    public RegisterEndpoint(IAuthService authService)
    {
        _authService = authService;
    }

    public override void Configure()
    {
        Post("/api/auth/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterDto req, CancellationToken ct)
    {
        await _authService.RegisterAsync(req);
        await HttpContext.Response.WriteAsJsonAsync("User registered successfully", ct);
    }
}