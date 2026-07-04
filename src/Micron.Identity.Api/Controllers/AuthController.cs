using MediatR;
using Micron.Identity.Application.Common.Interfaces;
using Micron.Identity.Application.Features.Auth.Login;
using Microsoft.AspNetCore.Mvc;

namespace Micron.Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<TokenResult>> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Ok(result);
    }
}
