using System.ComponentModel.DataAnnotations;
using RequestManagement.Core.Enums;

namespace RequestManagement.Core.DTOs.Request;

public class CreateRequestDto
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Description is required")]
    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = null!;

    public Priority Priority { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be greater than 0")]
    public int CategoryId { get; set; }
}