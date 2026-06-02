using FastEndpoints;
using RequestManagement.Application.Interfaces;
using System.Security.Claims;

namespace RequestManagement.API.Endpoints.Request;

public class ChangeToInProgressRequest
{
    public int Id { get; set; }
}

public class ChangeToInProgressEndpoint : Endpoint<ChangeToInProgressRequest>
{
    private readonly IRequestService _requestService;

    public ChangeToInProgressEndpoint(IRequestService requestService)
    {
        _requestService = requestService;
    }

    public override void Configure()
    {
        Patch("/api/request/{id}/in-progress");
        Roles("Executor", "Admin");
    }

    public override async Task HandleAsync(ChangeToInProgressRequest req, CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _requestService.ChangeStatusToInProgressAsync(req.Id, userId);
        await HttpContext.Response.WriteAsJsonAsync("Status changed to InProgress", ct);
    }
}