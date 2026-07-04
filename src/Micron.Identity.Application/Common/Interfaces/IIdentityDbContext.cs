using Micron.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Micron.Identity.Application.Common.Interfaces;

public interface IIdentityDbContext
{
    DbSet<User> Users { get; }

    DbSet<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
