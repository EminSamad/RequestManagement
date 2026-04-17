using RequestManagement.Core.Enums;
namespace RequestManagement.Core.Entities;

public class Request : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Priority Priority { get; set; }
    public DateTime DueDate { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Initial;
    public string? FilePath { get; set; }
    
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    
    public int RequesterId { get; set; }
    public User Requester { get; set; }
    
    public int? ExecutorId { get; set; }
    public User? Executor { get; set; }
    
    public string? ResponseText { get; set; }
    public string? ResponseFilePath { get; set; }
}