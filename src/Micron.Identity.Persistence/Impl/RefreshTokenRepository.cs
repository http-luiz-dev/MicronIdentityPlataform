using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Micron.Identity.Domain.Entities;
using Micron.Identity.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace Micron.Identity.Persistence.Impl
{
    public class RefreshTokenRepository(IdentityDbContext context) : IRefreshTokenRepository
    {
        public async Task<Guid> Create(RefreshToken refreshToken)
        {
            await context.RefreshTokens.AddAsync(refreshToken);
            await context.SaveChangesAsync();
            return refreshToken.Id;
        }

        public async Task<RefreshToken?> FindById(Guid id) =>
            await context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Id == id);

        public async Task<RefreshToken?> FindByToken(string token) =>
            await context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);

        public async Task<IEnumerable<RefreshToken>> ListByUser(Guid userId) =>
            await context.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .ToListAsync();

        public async Task Update(RefreshToken refreshToken)
        {
            context.RefreshTokens.Update(refreshToken);
            await context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var refreshToken = await context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Id == id);
            if (refreshToken is not null)
            {
                context.RefreshTokens.Remove(refreshToken);
                await context.SaveChangesAsync();
            }
        }
    }
}
