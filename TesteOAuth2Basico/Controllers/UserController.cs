using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Model.DTO;
using TesteOAuth2Basico.Repository;



namespace TesteOAuth2Basico.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;
        public UserController(UserRepository user_Repository)
        {
            _userRepository = user_Repository;  
        }
        [HttpPost("insert-user")]
        public async Task<UserDTO> InsertUser(UserDTO googleUser)
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
                    return googleUserData;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
 