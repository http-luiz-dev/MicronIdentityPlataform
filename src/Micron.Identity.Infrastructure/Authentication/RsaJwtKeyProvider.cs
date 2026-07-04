using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Micron.Identity.Infrastructure.Authentication;

/// <summary>
/// Provides the asymmetric (RSA) key material used to sign and validate JWTs.
/// The issuing identity service loads the private key (which also yields the public key),
/// while downstream APIs load only the public key to validate tokens.
/// </summary>
public interface IJwtKeyProvider
{
    /// <summary>Public key used to validate token signatures. Always available.</summary>
    SecurityKey PublicKey { get; }

    /// <summary>Credentials used to sign tokens. Null when only the public key was loaded.</summary>
    SigningCredentials SigningCredentials { get; }

    /// <summary>The signature algorithm (RS256).</summary>
    string Algorithm { get; }
}

public sealed class RsaJwtKeyProvider : IJwtKeyProvider, IDisposable
{
    private readonly RSA _rsa;

    public SecurityKey PublicKey { get; }

    public SigningCredentials SigningCredentials { get; }

    public string Algorithm => SecurityAlgorithms.RsaSha256;

    private RsaJwtKeyProvider(RSA rsa, bool canSign)
    {
        _rsa = rsa;

        // A single RsaSecurityKey backed by the imported RSA. When the private key was
        // loaded it holds both halves, so the same instance both signs and validates.
        var key = new RsaSecurityKey(rsa) { KeyId = ComputeKeyId(rsa) };

        PublicKey = key;
        SigningCredentials = canSign
            ? new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
            : throw new NotSupportedException(
                "This service only has the public key and cannot sign tokens.");
    }

    private RsaJwtKeyProvider(RSA rsa)
    {
        _rsa = rsa;
        PublicKey = new RsaSecurityKey(rsa) { KeyId = ComputeKeyId(rsa) };
        SigningCredentials = null!;
    }

    /// <summary>
    /// Builds a provider from PEM files. When <paramref name="privateKeyPath"/> is set the
    /// service can sign (identity issuer); otherwise it loads only <paramref name="publicKeyPath"/>
    /// and can only validate (resource APIs).
    /// </summary>
    public static RsaJwtKeyProvider FromPemFiles(string? privateKeyPath, string? publicKeyPath)
    {
        if (!string.IsNullOrWhiteSpace(privateKeyPath))
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(ReadPem(privateKeyPath));
            return new RsaJwtKeyProvider(rsa, canSign: true);
        }

        if (!string.IsNullOrWhiteSpace(publicKeyPath))
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(ReadPem(publicKeyPath));
            return new RsaJwtKeyProvider(rsa);
        }

        throw new InvalidOperationException(
            "No JWT key configured. Set 'JwtSettings:PrivateKeyPath' (issuer) or 'JwtSettings:PublicKeyPath' (validator).");
    }

    private static string ReadPem(string path) =>
        File.Exists(path)
            ? File.ReadAllText(path)
            : throw new FileNotFoundException($"JWT key file not found: '{path}'.", path);

    // Stable key id (kid) derived from the public key, so tokens carry a header that lets
    // validators pick the right key once you rotate to multiple keys.
    private static string ComputeKeyId(RSA rsa)
    {
        var spki = rsa.ExportSubjectPublicKeyInfo();
        var hash = SHA256.HashData(spki);
        return Base64UrlEncoder.Encode(hash, 0, 8);
    }

    public void Dispose() => _rsa.Dispose();
}
