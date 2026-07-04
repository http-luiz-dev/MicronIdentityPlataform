using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Micron.Identity.Domain.Entities;

namespace Micron.Identity.Domain.Interfaces.Repository
{
    public interface ITenantRepository
    {
        Task<Guid> Create(Tenant tenant);
        Task<Tenant?> FindById(Guid id);
        Task<Tenant?> FindByName(string name);
        Task<IEnumerable<Tenant>> List(int page, int pageSize);
        Task Update(Tenant tenant);
        Task Delete(Guid id);
    }
}
