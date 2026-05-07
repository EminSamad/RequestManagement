using System.ComponentModel.DataAnnotations;

namespace RequestManagement.Core.DTOs.Auth;

public class InviteDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [Range(1, 3, ErrorMessage = "RoleId must be 1 (Admin), 2 (Requester) or 3 (Executor)")]
    public int RoleId { get; set; }
}