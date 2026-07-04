using Micron.Identity.Domain.Common;

namespace Micron.Identity.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public ICollection<RefreshToken> RefreshTokens { get; init; } = [];
}
