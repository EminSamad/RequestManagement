using RequestManagement.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace RequestManagement.Core.DTOs.Request;

public class RequestFilterDto
{
    public string? SearchText { get; set; }
    public int? CategoryId { get; set; }
    public Priority? Priority { get; set; }
    public RequestStatus? Status { get; set; }
    public string? OrderBy { get; set; } // "date", "status", "priority"
    public bool OrderByAsc { get; set; } = false;
    [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be greater than 0")]
    public int PageNumber { get; set; } = 1;
    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
    public int PageSize { get; set; } = 10;
}