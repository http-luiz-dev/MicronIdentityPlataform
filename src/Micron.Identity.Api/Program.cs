using Micron.Identity.Api.Grpc;
using Micron.Identity.Application;
using Micron.Identity.Infrastructure;
using Micron.Identity.Infrastructure.Authentication;
using Micron.Identity.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? throw new InvalidOperationException("Section 'JwtSettings' not found.");

string ResolveKeyPath(string path) =>
    Path.IsPathRooted(path) ? path : Path.Combine(builder.Environment.ContentRootPath, path);

// This service issues tokens, so it loads the PRIVATE key (to sign) — which also yields
// the PUBLIC key used to validate. Downstream APIs load only the public key (see README).
var jwtKeyProvider = RsaJwtKeyProvider.FromPemFiles(
    privateKeyPath: ResolveKeyPath(jwtSettings.PrivateKeyPath),
    publicKeyPath: string.IsNullOrWhiteSpace(jwtSettings.PublicKeyPath)
        ? null
        : ResolveKeyPath(jwtSettings.PublicKeyPath));

builder.Services.AddSingleton<IJwtKeyProvider>(jwtKeyProvider);

builder.Services
    .AddAuthentication("Asymmetric")
    .AddJwtBearer("Asymmetric", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = jwtKeyProvider.PublicKey,
            ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
        };
        return Task.CompletedTask;
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<IdentityRpcService>();

app.Run();
