using RequestManagement.Data.Context;
using RequestManagement.Data.Repositories.Interfaces;
using RequestManagement.Core.Entities;
using RequestManagement.Data.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

namespace RequestManagement.Data.Repositories.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IGenericRepository<User>? _users;
    private IGenericRepository<Role>? _roles;
    private IGenericRepository<Request>? _requests;
    private IGenericRepository<Category>? _categories;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<User> Users => _users ??= new GenericRepository<User>(_context);
    public IGenericRepository<Role> Roles => _roles ??= new GenericRepository<Role>(_context);
    public IGenericRepository<Request> Requests => _requests ??= new GenericRepository<Request>(_context);
    public IGenericRepository<Category> Categories => _categories ??= new GenericRepository<Category>(_context);

    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public void Dispose()
        => _context.Dispose();

        public async Task<User?> GetUserWithRolesAsync(string email)
{
    return await _context.Users
        .Include(u => u.UserRoles)
        .ThenInclude(ur => ur.Role)
        .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
}
}
