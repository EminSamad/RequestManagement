using FastEndpoints;
using RequestManagement.Application.Interfaces;
using System.Security.Claims;

namespace RequestManagement.API.Endpoints.Request;

public class RejectRequest { public int Id { get; set; } }

public class RejectEndpoint : Endpoint<RejectRequest>
{
    private readonly IRequestService _requestService;
    public RejectEndpoint(IRequestService requestService) => _requestService = requestService;

    public override void Configure()
    {
        Patch("/api/request/{id}/reject");
        Roles("Admin");
    }

    public override async Task HandleAsync(RejectRequest req, CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _requestService.RejectRequestAsync(req.Id, userId);
        await HttpContext.Response.WriteAsJsonAsync("Request rejected", ct);
    }
}