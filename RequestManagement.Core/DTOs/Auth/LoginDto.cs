using System.ComponentModel.DataAnnotations;

namespace RequestManagement.Core.DTOs.Auth;

public class LoginDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    [RegularExpression(@"^[^<>""'%;()&+]*$", ErrorMessage = "Invalid characters detected")]
    public string Title { get; set; } = null!;
}