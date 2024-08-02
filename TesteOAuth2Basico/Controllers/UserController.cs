using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Model.DTO;
using Services.Queries;
using StackExchange.Redis;
using TesteOAuth2Basico.Repository;
using TesteOAuth2Basico.Service.Commands;
using TesteOAuth2Basico.Services.Commands;


namespace TesteOAuth2Basico.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        //private readonly CreateUserCommandHandler _createUserHandler;
        //private readonly GetUserByIdQueryHandler _getUserByIdHandler;
        private readonly UserRepository _userRepository;
        private readonly ILogger<UserController> _logger;
        public UserController(UserRepository user_Repository, ILogger<UserController> logger)
        {
            //_createUserHandler = createUserHandler;
            //_getUserByIdHandler = getUserByIdHandler;
            _userRepository = user_Repository;
            _logger = logger;
        }

        [HttpPost("insert-user")]
        public async Task<IActionResult> InsertUser(UserDTO googleUser)
        {
            using (var sqlConnection = new SqlConnection("Data Source=127.0.0.1; Initial Catalog=DBTstOAUthBas; User Id=sa; Password=SqlServer2019!; TrustServerCertificate=Yes;"))
            {
                sqlConnection.Open();
                UserDTO googleUserData = new UserDTO
                {
                    IdUser = googleUser.IdUser,
                    Name = googleUser.Name,
                    Email = googleUser.Email,
                    ProfileImageUrl = googleUser.ProfileImageUrl
                };
                bool result = _userRepository.PostUserSql(googleUserData);
                if (result)
                {
                    return Ok("Usu�rio inserido com sucesso.");
                }
                else
                {
                    return BadRequest("Usu�rio j� existe ERRO AQUI.");
                }
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateUser(CreateUserCommand command)
        //{
        //    await _createUserHandler.Handle(command);
        //    return Ok();
        //}

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetUserById(string id)
        //{
        //    var query = new GetUserByIdQuery { UserId = id };
        //    var user = await _getUserByIdHandler.Handle(query);
        //    return Ok(user);
        //}

        //[HttpPost("insert-user")]
        //public async Task<IActionResult> InsertUser(GoogleUserData googleUser)
        //{
        //    try
        //    {
        //        if (googleUser == null || string.IsNullOrWhiteSpace(googleUser.Email) ||
        //            string.IsNullOrWhiteSpace(googleUser.AccessTokenGoogle) ||
        //            string.IsNullOrWhiteSpace(googleUser.RefreshTokenGoogle))
        //        {
        //            _logger.LogError("Dados do usu�rio inv�lidos.");
        //            return BadRequest("Dados do usu�rio inv�lidos.");
        //        }

        //        var result = await _userRepository.RegisterUserFromGoogle(googleUser);
        //        if (result)
        //        {
        //            _logger.LogInformation("Usu�rio inserido com sucesso.");
        //            return Ok("Usu�rio inserido com sucesso.");
        //        }
        //        else
        //        {
        //            _logger.LogWarning("Usu�rio j� existe.");
        //            return BadRequest("Usu�rio j� existe.");
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        _logger.LogError(ex, "Erro ao inserir usu�rio no banco de dados.");
        //        return StatusCode(500, "Ocorreu um erro interno na INSER��O. Tente novamente mais tarde.");
        //    }
        //    catch (Exception ex)
        //    {
        //       _logger.LogError(ex, "Erro ao inserir usu�rio no banco de dados.");
        //        return StatusCode(500, "Ocorreu um erro interno na INSER��O. Tente novamente mais tarde.");
        //    }
    }
}
 