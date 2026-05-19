namespace RequestManagement.Core.Entities;

public class UserRole
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
}