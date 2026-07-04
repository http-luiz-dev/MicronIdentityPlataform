using MediatR;
using Micron.Identity.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Micron.Identity.Application.Features.Auth.Login;

public class LoginCommandHandler(
    IIdentityDbContext dbContext,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService) : IRequestHandler<LoginCommand, TokenResult>
{
    public async Task<TokenResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!user.IsActive || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        return await jwtTokenService.GenerateTokensAsync(user, cancellationToken);
    }
}
