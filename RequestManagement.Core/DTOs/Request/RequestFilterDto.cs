using RequestManagement.Core.Enums;

namespace RequestManagement.Core.DTOs.Request;

public class RequestFilterDto
{
    public string? SearchText { get; set; }
    public int? CategoryId { get; set; }
    public Priority? Priority { get; set; }
    public RequestStatus? Status { get; set; }
}