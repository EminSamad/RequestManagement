using Microsoft.AspNetCore.Mvc;
using RequestManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace RequestManagement.Domain.DTOs.Request;

public class RequestFilterDto
{
    [FromQuery] public string? SearchText { get; set; }
    [FromQuery] public int? CategoryId { get; set; }
    [FromQuery] public Priority? Priority { get; set; }
    [FromQuery] public RequestStatus? Status { get; set; }
    [FromQuery] public string? OrderBy { get; set; }
    [FromQuery] public bool? OrderByAsc { get; set; } = false;
    [FromQuery] public int? PageNumber { get; set; } = 1;
    [FromQuery] public int? PageSize { get; set; } = 10;
}