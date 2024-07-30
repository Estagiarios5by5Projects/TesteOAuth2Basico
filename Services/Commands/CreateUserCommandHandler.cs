using Repositories.Utils;
using Repostories.Utils;
using TesteOAuth2Basico.Model;
using TesteOAuth2Basico.Service.Commands;

namespace TesteOAuth2Basico.Services.Commands
{
    public class CreateUserCommandHandler
    {
        private readonly IUserRepository _userRepository;

        public CreateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Handle(CreateUserCommand command)
        {
            var user = new User
            {
                Name = command.Name,
                Email = command.Email
            };

            await _userRepository.AddUserAsync(user);
        }
    }
}
