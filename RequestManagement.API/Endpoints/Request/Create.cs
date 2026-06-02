using FastEndpoints;
using RequestManagement.Application.Interfaces;
using RequestManagement.Domain.DTOs.Request;
using RequestManagement.API.Services;
using System.Security.Claims;

namespace RequestManagement.API.Endpoints.Request;

public class CreateRequestEndpoint : Endpoint<CreateRequestDto>
{
    private readonly IRequestService _requestService;
    private readonly FileService _fileService;

    public CreateRequestEndpoint(IRequestService requestService, FileService fileService)
    {
        _requestService = requestService;
        _fileService = fileService;
    }

    public override void Configure()
    {
        Post("/api/request/create");
        Roles("Requester", "Admin");
        AllowFileUploads();
    }

    public override async Task HandleAsync(CreateRequestDto req, CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        string? filePath = null;

        if (Files.Count > 0)
            filePath = await _fileService.SaveFileAsync(Files[0]);

        await _requestService.CreateRequestAsync(req, userId, filePath);
        await HttpContext.Response.WriteAsJsonAsync("Request created successfully", ct);
    }
}