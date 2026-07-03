using DDD.Domain.UserAggregate;
using DDD.Domain.UserAggregate.Interfaces;
using DDD.Shared.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DDD.Infrastructure.Persistence.Repositories;

[Injectable(ServiceLifetime.Scoped, typeof(IUserRepository))]
public sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(
            u => u.Email.Value == email.ToLowerInvariant().Trim(),
            cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(
            u => u.Email.Value == email.ToLowerInvariant().Trim(),
            cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetPagedAsync(
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}