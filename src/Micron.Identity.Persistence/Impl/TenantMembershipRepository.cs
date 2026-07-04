using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Micron.Identity.Domain.Entities;
using Micron.Identity.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace Micron.Identity.Persistence.Impl
{
    public class TenantMembershipRepository(IdentityDbContext context) : ITenantMembershipRepository
    {
        public async Task<Guid> Create(TenantMembership membership)
        {
            await context.TenantMemberships.AddAsync(membership);
            await context.SaveChangesAsync();
            return membership.Id;
        }

        public async Task<TenantMembership?> FindById(Guid id) =>
            await context.TenantMemberships.FirstOrDefaultAsync(tm => tm.Id == id);

        public async Task<IEnumerable<TenantMembership>> ListByUser(Guid userId) =>
            await context.TenantMemberships
                .Where(tm => tm.UserId == userId)
                .ToListAsync();

        public async Task<IEnumerable<TenantMembership>> ListByTenant(Guid tenantId) =>
            await context.TenantMemberships
                .Where(tm => tm.TenantId == tenantId)
                .ToListAsync();

        public async Task Update(TenantMembership membership)
        {
            context.TenantMemberships.Update(membership);
            await context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var membership = await context.TenantMemberships.FirstOrDefaultAsync(tm => tm.Id == id);
            if (membership is not null)
            {
                context.TenantMemberships.Remove(membership);
                await context.SaveChangesAsync();
            }
        }
    }
}
