using Grpc.Core;
using Micron.Identity.Application.Common.Interfaces;

namespace Micron.Identity.Api.Grpc;

public class IdentityRpcService(IJwtTokenService jwtTokenService) : IdentityRpc.IdentityRpcBase
{
    public override async Task<ValidateTokenResponse> ValidateToken(ValidateTokenRequest request, ServerCallContext context)
    {
        var result = await jwtTokenService.ValidateAccessTokenAsync(request.AccessToken, context.CancellationToken);

        return new ValidateTokenResponse
        {
            IsValid = result.IsValid,
            UserId = result.UserId?.ToString() ?? string.Empty,
            Email = result.Email ?? string.Empty,
            Error = result.Error ?? string.Empty,
        };
    }
}
