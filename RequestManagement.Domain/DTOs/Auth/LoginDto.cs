using System.ComponentModel.DataAnnotations;

namespace RequestManagement.Domain.DTOs.Auth;

public class LoginDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}