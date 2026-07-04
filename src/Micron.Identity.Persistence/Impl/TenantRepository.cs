using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Micron.Identity.Domain.Entities;
using Micron.Identity.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace Micron.Identity.Persistence.Impl
{
    public class TenantRepository(IdentityDbContext context) : ITenantRepository
    {
        public async Task<Guid> Create(Tenant tenant)
        {
            await context.Tenants.AddAsync(tenant);
            await context.SaveChangesAsync();
            return tenant.Id;
        }

        public async Task<Tenant?> FindById(Guid id) =>
            await context.Tenants
                .Include(t => t.Products)
                .FirstOrDefaultAsync(t => t.Id == id);

        public async Task<Tenant?> FindByName(string name) =>
            await context.Tenants
                .Include(t => t.Products)
                .FirstOrDefaultAsync(t => t.Name == name);

        public async Task<IEnumerable<Tenant>> List(int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;
            return await context.Tenants
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task Update(Tenant tenant)
        {
            context.Tenants.Update(tenant);
            await context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.Id == id);
            if (tenant is not null)
            {
                context.Tenants.Remove(tenant);
                await context.SaveChangesAsync();
            }
        }
    }
}
