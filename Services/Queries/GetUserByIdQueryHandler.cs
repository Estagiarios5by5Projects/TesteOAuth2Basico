using Model;
using Repositories.Utils;


namespace Services.Queries
{
    public class GetUserByIdQueryHandler
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<GoogleUserData> Handle(GetUserByIdQuery query)
        {
            return await _userRepository.GetUserByIdAsync(query.UserId);
        }
    }
}
