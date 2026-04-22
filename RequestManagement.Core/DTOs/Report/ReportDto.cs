namespace RequestManagement.Core.DTOs.Report;

public class ReportDto
{
    public int RequestId { get; set; }
    public string Category { get; set; } = null!;
    public string Priority { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public string? ResponseBy { get; set; }
    public DateTime? ResponseTime { get; set; }
    public string Status { get; set; } = null!;
}