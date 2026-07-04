using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Micron.Identity.Domain.Entities;

namespace Micron.Identity.Domain.Interfaces.Repository
{
    public interface IRoleRepository
    {
        Task<Guid> Create(Role role);
        Task<Role?> FindById(Guid id);
        Task<Role?> FindByName(string name);
        Task<IEnumerable<Role>> List(int page, int pageSize);
        Task Update(Role role);
        Task Delete(Guid id);
    }
}
