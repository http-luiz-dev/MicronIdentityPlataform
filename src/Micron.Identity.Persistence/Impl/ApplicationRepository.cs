using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Micron.Identity.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
// Aliased to avoid the clash with the 'Micron.Identity.Application' namespace.
using ApplicationEntity = Micron.Identity.Domain.Entities.Application;

namespace Micron.Identity.Persistence.Impl
{
    public class ApplicationRepository(IdentityDbContext context) : IApplicationRepository
    {
        public async Task<Guid> Create(ApplicationEntity application)
        {
            await context.Applications.AddAsync(application);
            await context.SaveChangesAsync();
            return application.Id;
        }

        public async Task<ApplicationEntity?> FindById(Guid id) =>
            await context.Applications.FirstOrDefaultAsync(a => a.Id == id);

        public async Task<ApplicationEntity?> FindByName(string name) =>
            await context.Applications.FirstOrDefaultAsync(a => a.Name == name);

        public async Task<IEnumerable<ApplicationEntity>> List(int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;
            return await context.Applications
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task Update(ApplicationEntity application)
        {
            context.Applications.Update(application);
            await context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var application = await context.Applications.FirstOrDefaultAsync(a => a.Id == id);
            if (application is not null)
            {
                context.Applications.Remove(application);
                await context.SaveChangesAsync();
            }
        }
    }
}
