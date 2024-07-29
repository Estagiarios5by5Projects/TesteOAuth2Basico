using Microsoft.Data.SqlClient;
using TesteOAuth2Basico.Model;
using TesteOAuth2Basico.Repository;

namespace TesteOAuth2Basico.Service
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        //POST: api/User
        //GET: api/User






    }
}
