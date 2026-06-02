using FastEndpoints;
using RequestManagement.Application.Interfaces;
using RequestManagement.Domain.DTOs.Auth;

namespace RequestManagement.API.Endpoints.Auth;

public class LoginEndpoint : Endpoint<LoginDto>
{
    private readonly IAuthService _authService;

    public LoginEndpoint(IAuthService authService)
    {
        _authService = authService;
    }

    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginDto req, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(req);
        HttpContext.Response.StatusCode = 200;
        await HttpContext.Response.WriteAsJsonAsync(result, ct);
    }
}