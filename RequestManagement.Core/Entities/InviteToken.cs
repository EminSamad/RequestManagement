namespace RequestManagement.Core.Entities;

public class InviteToken : BaseEntity
{
    public string Token { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public bool IsUsed { get; set; } = false;
    public DateTime ExpiresAt { get; set; }
}