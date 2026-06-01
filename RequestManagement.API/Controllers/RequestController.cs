using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RequestManagement.Application.Interfaces;
using RequestManagement.Core.DTOs.Request;
using System.Security.Claims;
using RequestManagement.API.Services;

namespace RequestManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RequestController : ControllerBase
{
    private readonly IRequestService _requestService;
    private readonly FileService _fileService;
    private readonly ILogger<RequestController> _logger;

    public RequestController(IRequestService requestService, FileService fileService, ILogger<RequestController> logger)
    {
        _requestService = requestService;
        _fileService = fileService;
        _logger = logger;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("my-requests")]
    [Authorize(Roles = "Requester,Admin")]
    public async Task<IActionResult> GetMyRequests()
    {
        _logger.LogInformation("User {UserId} fetching their requests", GetUserId());
        var result = await _requestService.GetMyRequestsAsync(GetUserId());
        return Ok(result);
    }

    [HttpGet("requests-to-me")]
    [Authorize(Roles = "Executor,Admin")]
    public async Task<IActionResult> GetRequestsToMe()
    {
        _logger.LogInformation("User {UserId} fetching requests assigned to them", GetUserId());
        var result = await _requestService.GetRequestsToMeAsync(GetUserId());
        return Ok(result);
    }

    [HttpPost("create")]
    [Authorize(Roles = "Requester,Admin")]  
    public async Task<IActionResult> Create([FromForm] CreateRequestDto dto, IFormFile? file)
    {
        _logger.LogInformation("User {UserId} creating a request", GetUserId());
        string? filePath = null;
        if (file != null)
            filePath = await _fileService.SaveFileAsync(file);
        await _requestService.CreateRequestAsync(dto, GetUserId(), filePath);
        _logger.LogInformation("Request created successfully by user {UserId}", GetUserId());
        return Ok("Request created successfully");
    }

    [HttpPost("respond")]
    [Authorize(Roles = "Executor,Admin")]
    public async Task<IActionResult> Respond(ResponseRequestDto dto)
    {
        _logger.LogInformation("User {UserId} responding to request {RequestId}", GetUserId(), dto.RequestId);
        await _requestService.RespondToRequestAsync(dto, GetUserId());
        _logger.LogInformation("User {UserId} responded to request {RequestId}", GetUserId(), dto.RequestId);
        return Ok("Response submitted successfully");
    }

    [HttpPatch("{id}/in-progress")]
    [Authorize(Roles = "Executor,Admin")]
    public async Task<IActionResult> ChangeToInProgress(int id)
    {
        _logger.LogInformation("User {UserId} changing request {RequestId} to InProgress", GetUserId(), id);
        await _requestService.ChangeStatusToInProgressAsync(id, GetUserId());
        _logger.LogInformation("Request {RequestId} changed to InProgress by user {UserId}", id, GetUserId());
        return Ok("Status changed to InProgress");
    }

    [HttpPatch("{id}/complete")]
    [Authorize(Roles = "Executor,Admin")]
    public async Task<IActionResult> Complete(int id)
    {
        _logger.LogInformation("User {UserId} completing request {RequestId}", GetUserId(), id);
        await _requestService.CompleteRequestAsync(id, GetUserId());
        _logger.LogInformation("Request {RequestId} completed by user {UserId}", id, GetUserId());
        return Ok("Request completed");
    }

    [HttpPatch("{id}/approve")]
    [Authorize(Roles = "Requester,Admin")]
    public async Task<IActionResult> Approve(int id)
    {
        _logger.LogInformation("User {UserId} approving request {RequestId}", GetUserId(), id);
        await _requestService.ApproveRequestAsync(id, GetUserId());
        _logger.LogInformation("Request {RequestId} approved by user {UserId}", id, GetUserId());
        return Ok("Request approved");
    }

    [HttpPatch("{id}/decline")]
    [Authorize(Roles = "Requester,Admin")]
    public async Task<IActionResult> Decline(int id)
    {
        _logger.LogInformation("User {UserId} declining request {RequestId}", GetUserId(), id);
        await _requestService.DeclineRequestAsync(id, GetUserId());
        _logger.LogInformation("Request {RequestId} declined by user {UserId}", id, GetUserId());
        return Ok("Request declined");
    }

    [HttpPatch("{id}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reject(int id)
    {
        _logger.LogInformation("User {UserId} rejecting request {RequestId}", GetUserId(), id);
        await _requestService.RejectRequestAsync(id, GetUserId());
        _logger.LogInformation("Request {RequestId} rejected by user {UserId}", id, GetUserId());
        return Ok("Request rejected");
    }

    [HttpGet()]
    [Authorize(Roles = "Admin,Requester,Executor")]
    public async Task<IActionResult> GetFilteredRequests([FromQuery] RequestFilterDto filter)
    {
        _logger.LogInformation("User {UserId} filtering requests", GetUserId());
        var result = await _requestService.GetFilteredRequestsAsync(filter);
        return Ok(result);
    }
}