using DDD.Domain.UserAggregate;
using DDD.Domain.UserAggregate.Services;
using DDD.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DDD.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        await context.Database.EnsureCreatedAsync();

        if (await context.Users.AnyAsync())
            return;

        var salt = passwordHasher.GenerateSalt();
        var adminPassword = passwordHasher.Hash("admin123", salt);

        var admin = User.Create("管理员", "admin@ddd.com", adminPassword, salt, UserRole.Admin);
        context.Users.Add(admin);

        var userSalt = passwordHasher.GenerateSalt();
        var userPassword = passwordHasher.Hash("user123", userSalt);

        var user = User.Create("普通用户", "user@ddd.com", userPassword, userSalt, UserRole.User);
        context.Users.Add(user);

        await context.SaveChangesAsync();
    }
}