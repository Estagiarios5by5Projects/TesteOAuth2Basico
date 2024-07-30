using Repositories.Utils;
using TesteOAuth2Basico.Model;


namespace Services.Queries
{
    public class GetUserByIdQueryHandler
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> Handle(GetUserByIdQuery query)
        {
            return await _userRepository.GetUserByIdAsync(query.UserId);
        }
    }
}
