using Domain.Commands.InsertToken;
using Domain.Queries.ValidateToken;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;

namespace TesteOAuth2Basico.Controllers
{
    public class GoogleOAuthController : Controller
    {
        private readonly IMediator _mediator;
        public GoogleOAuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("validate-token")]
        public async Task<IActionResult> ValidateToken(string accessToken)
        {
            var query = new ValidateTokenQuery { AccessToken = accessToken };
            var isValidToken = await _mediator.Send(query);
            if (isValidToken == true )
            {
                return Ok("Token de acesso é válido.");
            }
            else
            {
                return Unauthorized("Token de acesso não é válido.");
            }
        }

        [HttpPost("insert-token-redis")]
        public async Task<IActionResult> InsertTokenRedis(TokenDTO tokenUser)
        {
            var command = new InsertTokenCommand { Token = tokenUser };
            var token = await _mediator.Send(command);
            if (token)
            {
                return Ok("Token de acesso inserido no Redis com sucesso.");
            }
            else
            {
                return BadRequest("Token de acesso já existe.");
            }
        }
    }
}

