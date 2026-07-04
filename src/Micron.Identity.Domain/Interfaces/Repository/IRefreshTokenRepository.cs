using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Micron.Identity.Domain.Entities;

namespace Micron.Identity.Domain.Interfaces.Repository
{
    public interface IRefreshTokenRepository
    {
        Task<Guid> Create(RefreshToken refreshToken);
        Task<RefreshToken?> FindById(Guid id);
        Task<RefreshToken?> FindByToken(string token);
        Task<IEnumerable<RefreshToken>> ListByUser(Guid userId);
        Task Update(RefreshToken refreshToken);
        Task Delete(Guid id);
    }
}
