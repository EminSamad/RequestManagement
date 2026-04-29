using System.ComponentModel.DataAnnotations;

namespace RequestManagement.Core.DTOs.Auth;

public class LoginDto
{
    [Required]
    [RegularExpression(@"^[^<>""'%;()&+]*$", ErrorMessage = "Invalid characters detected")]
    public string Email { get; set; } = null!;

    [Required]
    [RegularExpression(@"^[^<>""'%;()&+]*$", ErrorMessage = "Invalid characters detected")]
    public string Password { get; set; } = null!;
}