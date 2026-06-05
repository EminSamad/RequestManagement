using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace RequestManagement.Domain.DTOs.Report;

public class ReportFilterDto
{
    [FromQuery] public string? SearchText { get; set; }
    [FromQuery] public int? CategoryId { get; set; }
    [FromQuery] public string? Priority { get; set; }
    [FromQuery] public string? Status { get; set; }
    [FromQuery] public DateTime? StartDate { get; set; }
    [FromQuery] public DateTime? EndDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate.HasValue && EndDate.HasValue && StartDate > EndDate)
            yield return new ValidationResult("StartDate cannot be greater than EndDate");
    }
}