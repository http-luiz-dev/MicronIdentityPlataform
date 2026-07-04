using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Micron.Identity.Domain.Entities;

namespace Micron.Identity.Domain.Interfaces.Repository
{
    public interface ITenantMembershipRepository
    {
        Task<Guid> Create(TenantMembership membership);
        Task<TenantMembership?> FindById(Guid id);
        Task<IEnumerable<TenantMembership>> ListByUser(Guid userId);
        Task<IEnumerable<TenantMembership>> ListByTenant(Guid tenantId);
        Task Update(TenantMembership membership);
        Task Delete(Guid id);
    }
}
