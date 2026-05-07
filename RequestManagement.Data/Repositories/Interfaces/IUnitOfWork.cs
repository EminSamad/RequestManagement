using RequestManagement.Core.Entities;

namespace RequestManagement.Data.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<User> Users { get; }
    IGenericRepository<Role> Roles { get; }
    IGenericRepository<Request> Requests { get; }
    IGenericRepository<Category> Categories { get; }
    IGenericRepository<RefreshToken> RefreshTokens { get; }
    IGenericRepository<UserRole> UserRoles { get; }
    IGenericRepository<InviteToken> InviteTokens { get; }
    Task<User?> GetUserWithRolesAsync(string email);
    Task<User?> GetUserWithRolesByIdAsync(int userId);
    Task<int> SaveChangesAsync();

}