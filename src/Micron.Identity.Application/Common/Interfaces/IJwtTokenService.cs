using Micron.Identity.Domain.Entities;

namespace Micron.Identity.Application.Common.Interfaces;

public record TokenResult(string AccessToken, string RefreshToken, DateTimeOffset AccessTokenExpiresAtUtc);

public record TokenValidationResult(bool IsValid, Guid? UserId, string? Email, string? Error);

public interface IJwtTokenService
{
    Task<TokenResult> GenerateTokensAsync(User user, CancellationToken cancellationToken = default);

    Task<TokenResult> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task<TokenValidationResult> ValidateAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default);
}
