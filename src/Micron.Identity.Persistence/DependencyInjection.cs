using Micron.Identity.Application.Common.Interfaces;
using Micron.Identity.Domain.Interfaces.Repository;
using Micron.Identity.Persistence.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micron.Identity.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentityConnection")
            ?? throw new InvalidOperationException("Connection string 'IdentityConnection' not found.");

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IIdentityDbContext>(sp => sp.GetRequiredService<IdentityDbContext>());

        services.AddRepositories();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<ITenantMembershipRepository, TenantMembershipRepository>();

        return services;
    }
}
