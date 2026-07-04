using MediatR;
using Micron.Identity.Application.Common.Interfaces;

namespace Micron.Identity.Application.Features.Auth.Login;

public record LoginCommand(string Email, string Password) : IRequest<TokenResult>;
