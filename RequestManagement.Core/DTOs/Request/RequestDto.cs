using RequestManagement.Core.Enums;
namespace RequestManagement.Core.DTOs.Request;
public class RequestDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Priority { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; }
    public int CategoryId { get; set; }
    public string? FilePath { get; set; }
    public int RequesterId { get; set; }
    public int? ExecutorId { get; set; }
}