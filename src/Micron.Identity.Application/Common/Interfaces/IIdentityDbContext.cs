using Micron.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Micron.Identity.Application.Common.Interfaces;

public interface IIdentityDbContext
{
    DbSet<User> Users { get; }

    DbSet<RefreshToken> RefreshTokens { get; }

    DbSet<Role> Roles { get; }

    // Fully qualified: within this project 'Application' would otherwise bind to the
    // 'Micron.Identity.Application' namespace instead of the entity.
    DbSet<Micron.Identity.Domain.Entities.Application> Applications { get; }

    DbSet<Tenant> Tenants { get; }

    DbSet<TenantMembership> TenantMemberships { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
