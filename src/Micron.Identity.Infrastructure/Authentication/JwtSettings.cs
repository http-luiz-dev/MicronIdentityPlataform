namespace Micron.Identity.Infrastructure.Authentication;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    /// <summary>Path to the RSA private key (PKCS#8 PEM). Only the issuing identity service sets this.</summary>
    public string PrivateKeyPath { get; set; } = string.Empty;

    /// <summary>Path to the RSA public key (SPKI PEM). Every service that validates tokens needs this.</summary>
    public string PublicKeyPath { get; set; } = string.Empty;

    public int AccessTokenExpirationMinutes { get; set; } = 15;

    public int RefreshTokenExpirationDays { get; set; } = 7;
}
