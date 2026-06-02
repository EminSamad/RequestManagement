using FastEndpoints;
using RequestManagement.Application.Interfaces;
using RequestManagement.Domain.DTOs.Request;
using System.Security.Claims;

namespace RequestManagement.API.Endpoints.Request;

public class RespondEndpoint : Endpoint<ResponseRequestDto>
{
    private readonly IRequestService _requestService;

    public RespondEndpoint(IRequestService requestService)
    {
        _requestService = requestService;
    }

    public override void Configure()
    {
        Post("/api/request/respond");
        Roles("Executor", "Admin");
    }

    public override async Task HandleAsync(ResponseRequestDto req, CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _requestService.RespondToRequestAsync(req, userId);
        await HttpContext.Response.WriteAsJsonAsync("Response submitted successfully", ct);
    }
}