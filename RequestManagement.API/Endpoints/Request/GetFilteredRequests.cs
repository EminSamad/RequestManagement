using FastEndpoints;
using RequestManagement.Application.Interfaces;
using RequestManagement.Domain.DTOs.Request;

namespace RequestManagement.API.Endpoints.Request;

public class GetFilteredRequestsEndpoint : Endpoint<RequestFilterDto>
{
    private readonly IRequestService _requestService;

    public GetFilteredRequestsEndpoint(IRequestService requestService)
    {
        _requestService = requestService;
    }

    public override void Configure()
    {
        Get("/api/request");
        Roles("Admin", "Requester", "Executor");
    }

    public override async Task HandleAsync(RequestFilterDto req, CancellationToken ct)
    {
        var result = await _requestService.GetFilteredRequestsAsync(req);
        await HttpContext.Response.WriteAsJsonAsync(result, ct);
    }
}