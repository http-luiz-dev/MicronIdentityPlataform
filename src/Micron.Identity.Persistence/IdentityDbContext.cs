using Micron.Identity.Application.Common.Interfaces;
using Micron.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
// Aliased to avoid the clash with the 'Micron.Identity.Application' namespace.
using ApplicationEntity = Micron.Identity.Domain.Entities.Application;

namespace Micron.Identity.Persistence;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options)
    : DbContext(options), IIdentityDbContext
{
    public DbSet<User> Users => Set<User>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<ApplicationEntity> Applications => Set<ApplicationEntity>();

    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<TenantMembership> TenantMemberships => Set<TenantMembership>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
