using System.ComponentModel.DataAnnotations;
using RequestManagement.Domain.Enums;

namespace RequestManagement.Domain.DTOs.Request;

public class CreateRequestDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Priority Priority { get; set; }
    public DateTime DueDate { get; set; }
    public int CategoryId { get; set; }
}