using FastEndpoints;
using RequestManagement.Application.Interfaces;
using System.Security.Claims;

namespace RequestManagement.API.Endpoints.Request;

public class GetRequestsToMeEndpoint : EndpointWithoutRequest
{
    private readonly IRequestService _requestService;

    public GetRequestsToMeEndpoint(IRequestService requestService)
    {
        _requestService = requestService;
    }

    public override void Configure()
    {
        Get("/api/request/requests-to-me");
        Roles("Executor", "Admin");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _requestService.GetRequestsToMeAsync(userId);
        await HttpContext.Response.WriteAsJsonAsync(result, ct);
    }
}