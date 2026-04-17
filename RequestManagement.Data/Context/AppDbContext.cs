using Microsoft.EntityFrameworkCore;
using RequestManagement.Core.Entities;
using RequestManagement.Core.Enums;

namespace RequestManagement.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // UserRole many-to-many key
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        // Request - Requester
        modelBuilder.Entity<Request>()
            .HasOne(r => r.Requester)
            .WithMany(u => u.CreatedRequests)
            .HasForeignKey(r => r.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        // Request - Executor
        modelBuilder.Entity<Request>()
            .HasOne(r => r.Executor)
            .WithMany()
            .HasForeignKey(r => r.ExecutorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Soft delete filter
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<Request>().HasQueryFilter(r => !r.IsDeleted);
        modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Role>().HasQueryFilter(r => !r.IsDeleted);

        base.OnModelCreating(modelBuilder);
    }
}