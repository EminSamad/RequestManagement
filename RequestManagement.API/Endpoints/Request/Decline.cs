using FastEndpoints;
using RequestManagement.Application.Interfaces;
using System.Security.Claims;

namespace RequestManagement.API.Endpoints.Request;

public class DeclineRequest { public int Id { get; set; } }

public class DeclineEndpoint : Endpoint<DeclineRequest>
{
    private readonly IRequestService _requestService;
    public DeclineEndpoint(IRequestService requestService) => _requestService = requestService;

    public override void Configure()
    {
        Patch("/api/request/{id}/decline");
        Roles("Requester", "Admin");
    }

    public override async Task HandleAsync(DeclineRequest req, CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _requestService.DeclineRequestAsync(req.Id, userId);
        await HttpContext.Response.WriteAsJsonAsync("Request declined", ct);
    }
}