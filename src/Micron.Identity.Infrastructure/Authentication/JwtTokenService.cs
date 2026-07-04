using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Micron.Identity.Application.Common.Interfaces;
using Micron.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using AppTokenValidationResult = Micron.Identity.Application.Common.Interfaces.TokenValidationResult;

namespace Micron.Identity.Infrastructure.Authentication;

public class JwtTokenService(
    IOptions<JwtSettings> jwtOptions,
    IIdentityDbContext dbContext,
    ICacheService cacheService,
    IJwtKeyProvider keyProvider) : IJwtTokenService
{
    private readonly JwtSettings _settings = jwtOptions.Value;

    public async Task<TokenResult> GenerateTokensAsync(User user, CancellationToken cancellationToken = default)
    {
        var (accessToken, expiresAtUtc) = CreateAccessToken(user);
        var refreshTokenValue = GenerateRefreshTokenValue();

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddDays(_settings.RefreshTokenExpirationDays),
        };

        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await cacheService.SetAsync(
            CacheKey(refreshTokenValue),
            user.Id,
            TimeSpan.FromDays(_settings.RefreshTokenExpirationDays),
            cancellationToken);

        return new TokenResult(accessToken, refreshTokenValue, expiresAtUtc);
    }

    public async Task<TokenResult> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.RefreshTokens
            .Include(rt => rt.User)
            .SingleOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

        if (existing is null || !existing.IsActive || existing.User is null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        existing.RevokedAtUtc = DateTimeOffset.UtcNow;
        await cacheService.RemoveAsync(CacheKey(refreshToken), cancellationToken);

        return await GenerateTokensAsync(existing.User, cancellationToken);
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.RefreshTokens
            .SingleOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

        if (existing is not null)
        {
            existing.RevokedAtUtc = DateTimeOffset.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        await cacheService.RemoveAsync(CacheKey(refreshToken), cancellationToken);
    }

    public Task<AppTokenValidationResult> ValidateAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _settings.Issuer,
            ValidateAudience = true,
            ValidAudience = _settings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = keyProvider.PublicKey,
            ValidAlgorithms = [keyProvider.Algorithm],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
        };

        try
        {
            var principal = new JwtSecurityTokenHandler().ValidateToken(accessToken, validationParameters, out _);

            var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var email = principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

            return Task.FromResult(new AppTokenValidationResult(true, Guid.Parse(userId!), email, null));
        }
        catch (Exception ex)
        {
            return Task.FromResult(new AppTokenValidationResult(false, null, null, ex.Message));
        }
    }

    private (string Token, DateTimeOffset ExpiresAtUtc) CreateAccessToken(User user)
    {
        var expiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("display_name", user.DisplayName),
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAtUtc.UtcDateTime,
            signingCredentials: keyProvider.SigningCredentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }

    private static string GenerateRefreshTokenValue() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    private static string CacheKey(string refreshToken) => $"refresh-token:{refreshToken}";
}
