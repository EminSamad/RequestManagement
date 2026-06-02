using FastEndpoints;
using RequestManagement.Application.Interfaces;
using System.Security.Claims;

namespace RequestManagement.API.Endpoints.Request;

public class GetMyRequestsEndpoint : EndpointWithoutRequest
{
    private readonly IRequestService _requestService;

    public GetMyRequestsEndpoint(IRequestService requestService)
    {
        _requestService = requestService;
    }

    public override void Configure()
    {
        Get("/api/request/my-requests");
        Roles("Requester", "Admin");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _requestService.GetMyRequestsAsync(userId);
        await HttpContext.Response.WriteAsJsonAsync(result, ct);
    }
}