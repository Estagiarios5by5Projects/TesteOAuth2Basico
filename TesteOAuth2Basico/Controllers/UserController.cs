using Domain.Commands.CreateUser;
using Domain.Queries.GetUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TesteOAuth2Basico.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("insert-user")]
        public async Task<IActionResult> InsertUser(CreateUserCommand googleUser)
        {
            var user = await _mediator.Send(googleUser);
            if (user == null)
            {
                return BadRequest("Usuário não foi inserido.");
            }
            else
            {
                return Ok(user);
            }
        }

        [HttpGet("get-user")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _mediator.Send(new GetUserByEmailQuery { Email = email });
            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }
            else
            {
                return Ok(user);
            }
        }
    }
}

