using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Micron.Identity.Domain.Entities;

namespace Micron.Identity.Domain.Interfaces.Repository
{
    public interface IApplicationRepository
    {
        Task<Guid> Create(Application application);
        Task<Application?> FindById(Guid id);
        Task<Application?> FindByName(string name);
        Task<IEnumerable<Application>> List(int page, int pageSize);
        Task Update(Application application);
        Task Delete(Guid id);
    }
}
