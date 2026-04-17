namespace RequestManagement.Core.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public  ICollection<UserRole> UserRoles { get; set; }
        public  ICollection<Request> CreatedRequests { get; set; }
    }
}