using FastEndpoints;
using RequestManagement.Application.Interfaces;
using RequestManagement.Domain.DTOs.User;

namespace RequestManagement.API.Endpoints.Auth;

public class RegisterWithTokenRequest
{
    public RegisterDto Dto { get; set; } = default!;
    public string Token { get; set; } = default!;
}

public class RegisterWithTokenEndpoint : Endpoint<RegisterWithTokenRequest>
{
    private readonly IAuthService _authService;

    public RegisterWithTokenEndpoint(IAuthService authService)
    {
        _authService = authService;
    }

    public override void Configure()
    {
        Post("/api/auth/register-with-token");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterWithTokenRequest req, CancellationToken ct)
    {
        await _authService.RegisterWithTokenAsync(req.Dto, req.Token);
        await HttpContext.Response.WriteAsJsonAsync("User registered successfully", ct);
    }
}