using RequestManagement.Core.Entities;

namespace RequestManagement.Data.Repositories.Interfaces;
    public interface IUnitOfWork : IDisposable
    {
    IGenericRepository<User> Users { get; }
    IGenericRepository<Role> Roles { get; }
    IGenericRepository<Request> Requests { get; }
    IGenericRepository<Category> Categories { get; }
    
    Task<int> SaveChangesAsync();

    }