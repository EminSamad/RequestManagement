using System.ComponentModel.DataAnnotations;

namespace RequestManagement.Core.DTOs.Report;

public class ReportFilterDto
{
    public string? SearchText { get; set; }
    public int? CategoryId { get; set; }
    public string? Priority { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate.HasValue && EndDate.HasValue && StartDate > EndDate)
            yield return new ValidationResult("StartDate cannot be greater than EndDate");
    }
}