using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Micron.Identity.Domain.Entities;

namespace Micron.Identity.Domain.Interfaces.Repository
{
    public interface IUserRepository 
    {
        Task<Guid> Create(User user);
        Task<User?> FindById(Guid id);
        Task<User?> FindByEmail(string email);
        Task<IEnumerable<User>> List(int page, int pageSize);
        Task Update(User user);
        Task Delete(Guid id);

    }
}