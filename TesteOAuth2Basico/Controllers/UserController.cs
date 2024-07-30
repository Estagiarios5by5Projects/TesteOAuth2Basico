using Microsoft.AspNetCore.Mvc;
using Services.Queries;
using TesteOAuth2Basico.Service.Commands;
using TesteOAuth2Basico.Services.Commands;


namespace TesteOAuth2Basico.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly CreateUserCommandHandler _createUserHandler;
        private readonly GetUserByIdQueryHandler _getUserByIdHandler;

        public UserController(CreateUserCommandHandler createUserHandler, GetUserByIdQueryHandler getUserByIdHandler)
        {
            _createUserHandler = createUserHandler;
            _getUserByIdHandler = getUserByIdHandler;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserCommand command)
        {
            await _createUserHandler.Handle(command);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var query = new GetUserByIdQuery { UserId = id };
            var user = await _getUserByIdHandler.Handle(query);
            return Ok(user);
        }
    }
}
