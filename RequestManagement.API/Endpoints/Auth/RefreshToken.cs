using FastEndpoints;
using RequestManagement.Application.Interfaces;

namespace RequestManagement.API.Endpoints.Auth;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = default!;
}

public class RefreshTokenEndpoint : Endpoint<RefreshTokenRequest>
{
    private readonly IAuthService _authService;

    public RefreshTokenEndpoint(IAuthService authService)
    {
        _authService = authService;
    }

    public override void Configure()
    {
        Post("/api/auth/refresh-token");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RefreshTokenRequest req, CancellationToken ct)
    {
        var result = await _authService.RefreshTokenAsync(req.RefreshToken);
        await HttpContext.Response.WriteAsJsonAsync(result, ct);
    }
}