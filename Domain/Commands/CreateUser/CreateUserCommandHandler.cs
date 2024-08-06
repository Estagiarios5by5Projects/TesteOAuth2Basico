using MediatR;
using Repositories.Utils;

namespace Domain.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public CreateUserCommandHandler(IUserRepository user_Repository)
        {
            _userRepository = user_Repository;
        }

        public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            return await _userRepository.AddUserAsync(request.User);
        }

        //public async Task<UserDTO> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        //{
        //    var newUser = new UserDTO
        //    {
        //        Name = request.Name,
        //        Email = request.Email,
        //        ProfileImageUrl = request.ProfileImageUrl
        //    };

        //    bool result = _userRepository.PostUserSql(newUser);
        //    if (result)
        //    {
        //        return newUser;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

    }
}
