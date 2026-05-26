using Microsoft.EntityFrameworkCore;
using RequestManagement.Core.Entities;
using RequestManagement.Data.Context;
using Microsoft.AspNetCore.Identity;

namespace RequestManagement.Data.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        // Roles
        if (!await context.Roles.AnyAsync())
        {
            var adminRole = new Role
            {
                Name = "Admin"
            };

            var userRole = new Role
            {
                Name = "User"
            };

            await context.Roles.AddRangeAsync(adminRole, userRole);
            await context.SaveChangesAsync();
        }

        // Admin user
        var adminEmail = "admin@test.com";

        var adminUser = await context.Users
            .Include(x => x.UserRoles)
            .FirstOrDefaultAsync(x => x.Email == adminEmail);

        if (adminUser == null)
        {
            var passwordHasher = new PasswordHasher<User>();

            adminUser = new User
            {
                FullName = "System Admin",
                Email = adminEmail
            };

            adminUser.PasswordHash =
                passwordHasher.HashPassword(adminUser, "Admin123!");

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();
        }

        // Assign Admin role
        var adminRoleEntity = await context.Roles
            .FirstAsync(x => x.Name == "Admin");

        var hasRole = await context.UserRoles.AnyAsync(x =>
            x.UserId == adminUser.Id &&
            x.RoleId == adminRoleEntity.Id);

        if (!hasRole)
        {
            await context.UserRoles.AddAsync(new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRoleEntity.Id
            });

            await context.SaveChangesAsync();
        }
    }
}