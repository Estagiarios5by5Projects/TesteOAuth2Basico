using Model;
using Repositories.Utils;
using TesteOAuth2Basico.Service.Commands;

namespace TesteOAuth2Basico.Services.Commands
{
    public class CreateUserCommandHandler
    {
        private readonly IUserRepository _userRepository;

        //public CreateUserCommandHandler(IUserRepository userRepository)
        //{
        //    _userRepository = userRepository;
        //}

        public async Task Handle(CreateUserCommand command)
        {
            var user = new GoogleUserData
            {
                Name = command.Name,
                Email = command.Email
            };

            await _userRepository.AddUserAsync(user);
        }
    }
}
