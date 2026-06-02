using FastEndpoints;
using RequestManagement.Application.Interfaces;
using System.Security.Claims;

namespace RequestManagement.API.Endpoints.Request;

public class CompleteRequest { public int Id { get; set; } }

public class CompleteEndpoint : Endpoint<CompleteRequest>
{
    private readonly IRequestService _requestService;
    public CompleteEndpoint(IRequestService requestService) => _requestService = requestService;

    public override void Configure()
    {
        Patch("/api/request/{id}/complete");
        Roles("Executor", "Admin");
    }

    public override async Task HandleAsync(CompleteRequest req, CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _requestService.CompleteRequestAsync(req.Id, userId);
        await HttpContext.Response.WriteAsJsonAsync("Request completed", ct);
    }
}