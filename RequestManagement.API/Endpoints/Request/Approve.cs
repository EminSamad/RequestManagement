using FastEndpoints;
using RequestManagement.Application.Interfaces;
using System.Security.Claims;

namespace RequestManagement.API.Endpoints.Request;

public class ApproveRequest { public int Id { get; set; } }

public class ApproveEndpoint : Endpoint<ApproveRequest>
{
    private readonly IRequestService _requestService;
    public ApproveEndpoint(IRequestService requestService) => _requestService = requestService;

    public override void Configure()
    {
        Patch("/api/request/{id}/approve");
        Roles("Requester", "Admin");
    }

    public override async Task HandleAsync(ApproveRequest req, CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _requestService.ApproveRequestAsync(req.Id, userId);
        await HttpContext.Response.WriteAsJsonAsync("Request approved", ct);
    }
}