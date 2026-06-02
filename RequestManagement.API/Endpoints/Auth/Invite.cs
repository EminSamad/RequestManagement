using FastEndpoints;
using RequestManagement.Application.Interfaces;
using RequestManagement.Domain.DTOs.Auth;

namespace RequestManagement.API.Endpoints.Auth;

public class InviteEndpoint : Endpoint<InviteDto>
{
    private readonly IAuthService _authService;

    public InviteEndpoint(IAuthService authService)
    {
        _authService = authService;
    }

    public override void Configure()
    {
        Post("/api/auth/invite");
        Roles("Admin");
    }

    public override async Task HandleAsync(InviteDto req, CancellationToken ct)
    {
        await _authService.InviteUserAsync(req.Email, req.RoleId);
        await HttpContext.Response.WriteAsJsonAsync("Invitation sent successfully", ct);
    }
}