using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Micron.Identity.Domain.Entities;
using Micron.Identity.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace Micron.Identity.Persistence.Impl
{
    public class RoleRepository(IdentityDbContext context) : IRoleRepository
    {
        public async Task<Guid> Create(Role role)
        {
            await context.Roles.AddAsync(role);
            await context.SaveChangesAsync();
            return role.Id;
        }

        public async Task<Role?> FindById(Guid id) =>
            await context.Roles.FirstOrDefaultAsync(r => r.Id == id);

        public async Task<Role?> FindByName(string name) =>
            await context.Roles.FirstOrDefaultAsync(r => r.Name == name);

        public async Task<IEnumerable<Role>> List(int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;
            return await context.Roles
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task Update(Role role)
        {
            context.Roles.Update(role);
            await context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var role = await context.Roles.FirstOrDefaultAsync(r => r.Id == id);
            if (role is not null)
            {
                context.Roles.Remove(role);
                await context.SaveChangesAsync();
            }
        }
    }
}
