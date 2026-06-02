using System.ComponentModel.DataAnnotations;

namespace RequestManagement.Domain.DTOs.Request;

public class ResponseRequestDto
{
    public int RequestId { get; set; }
    public string ResponseText { get; set; } = null!;
}