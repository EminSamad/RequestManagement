namespace RequestManagement.Core.DTOs.Report;

public class ReportFilterDto
{
    public string? SearchText { get; set; }
    public int? CategoryId { get; set; }
    public string? Priority { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}