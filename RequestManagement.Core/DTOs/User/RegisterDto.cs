namespace RequestManagement.Core.DTOs.User;

public class RegisterDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public String FullName { get; set; } = null!;
}