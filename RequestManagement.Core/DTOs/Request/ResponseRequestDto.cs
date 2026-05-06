using System.ComponentModel.DataAnnotations;

namespace RequestManagement.Core.DTOs.Request;

public class ResponseRequestDto
{
    [Required(ErrorMessage = "Request ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "RequestId must be greater than 0")]
    public int RequestId { get; set; }

    [Required(ErrorMessage = "Response text is required")]
    [MaxLength(1000, ErrorMessage = "Response text cannot exceed 1000 characters")]
    public string ResponseText { get; set; } = null!;
}