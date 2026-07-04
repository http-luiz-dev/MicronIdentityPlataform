using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Micron.Identity.Domain.Entities;
using Micron.Identity.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace Micron.Identity.Persistence.Impl
{
    public class UserRepository(IdentityDbContext context) : IUserRepository
    {
        public async Task<Guid> Create(User user)
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return user.Id;
        }

        public Task Delete(Guid id)
        {
            var user = context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                context.Users.Remove(user);
                context.SaveChanges();
            }
            return Task.CompletedTask;
        }

        public async Task<User?> FindByEmail(string email)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> FindById(Guid id)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
            await context.SaveChangesAsync();
            return user;
        }


        public Task<IEnumerable<User>> List(int page, int pageSize)
        {

            var skip = (page - 1) * pageSize;

            var users = context.Users
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            return Task.FromResult<IEnumerable<User>>(users);
        }

        public Task Update(User user)
        {
            context.Users.Update(user);
            context.SaveChanges();
            return Task.CompletedTask;
        }
    }
}