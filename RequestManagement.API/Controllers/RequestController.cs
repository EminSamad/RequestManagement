using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RequestManagement.Business.Interfaces;
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

    public RequestController(IRequestService requestService, FileService fileService)
    {
        _requestService = requestService;
        _fileService = fileService;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("my-requests")]
    public async Task<IActionResult> GetMyRequests()
    {
        var result = await _requestService.GetMyRequestsAsync(GetUserId());
        return Ok(result);
    }

    [HttpGet("requests-to-me")]
    public async Task<IActionResult> GetRequestsToMe()
    {
        var result = await _requestService.GetRequestsToMeAsync(GetUserId());
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromForm] CreateRequestDto dto, IFormFile? file)
    {
        string? filePath = null;
        if (file != null)
            filePath = await _fileService.SaveFileAsync(file);

        await _requestService.CreateRequestAsync(dto, GetUserId(), filePath);
        return Ok("Request created successfully");
    }

    [HttpPost("respond")]
    public async Task<IActionResult> Respond(ResponseRequestDto dto)
    {
        await _requestService.RespondToRequestAsync(dto, GetUserId());
        return Ok("Response submitted successfully");
    }

    [HttpPatch("{id}/in-progress")]
    public async Task<IActionResult> ChangeToInProgress(int id)
    {
        await _requestService.ChangeStatusToInProgressAsync(id, GetUserId());
        return Ok("Status changed to InProgress");
    }

    [HttpPatch("{id}/complete")]
    public async Task<IActionResult> Complete(int id)
    {
        await _requestService.CompleteRequestAsync(id, GetUserId());
        return Ok("Request completed");
    }

    [HttpPatch("{id}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        await _requestService.ApproveRequestAsync(id, GetUserId());
        return Ok("Request approved");
    }

    [HttpPatch("{id}/decline")]
    public async Task<IActionResult> Decline(int id)
    {
        await _requestService.DeclineRequestAsync(id, GetUserId());
        return Ok("Request declined");
    }

    [HttpPatch("{id}/reject")]
    public async Task<IActionResult> Reject(int id)
    {
        await _requestService.RejectRequestAsync(id, GetUserId());
        return Ok("Request rejected");
    }
}