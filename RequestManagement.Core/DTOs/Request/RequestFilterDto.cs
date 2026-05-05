using RequestManagement.Core.Enums;

namespace RequestManagement.Core.DTOs.Request;

public class RequestFilterDto
{
    public string? SearchText { get; set; }
    public int? CategoryId { get; set; }
    public Priority? Priority { get; set; }
    public RequestStatus? Status { get; set; }
    public string? OrderBy { get; set; } // "date", "status", "priority"
    public bool OrderByAsc { get; set; } = false;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}