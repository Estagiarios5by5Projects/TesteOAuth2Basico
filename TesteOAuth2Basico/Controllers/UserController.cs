using Domain.Commands;
using Domain.Queries;
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
        public async Task<IActionResult> GetUserById(string idUser)
        {
            //var query = new GetUserByIdQuery { IdUser = idUser };
            var user = await _mediator.Send(new GetUserByIdQuery { IdUser = idUser });
            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }
            else
            {
                return Ok(user);
            }
        }

        //public async Task<UserDTO> InsertUser(CreateUserComand googleUser)
        //using (var sqlConnection = new SqlConnection("Data Source=127.0.0.1; Initial Catalog=DBTstOAUthBas; User Id=sa; Password=SqlServer2019!; TrustServerCertificate=Yes;"))
        //{
        //    sqlConnection.Open();
        //    UserDTO googleUserData = new UserDTO
        //    {
        //        IdUser = googleUser.IdUser,
        //        Name = googleUser.Name,
        //        Email = googleUser.Email,
        //        ProfileImageUrl = googleUser.ProfileImageUrl
        //    };
        //    bool result = _userRepository.PostUserSql(googleUserData);
        //    if (result)
        //    {
        //        return googleUserData;
        //    }
        //    else
        //    {
        //        return null;
        //    }

    }
}

