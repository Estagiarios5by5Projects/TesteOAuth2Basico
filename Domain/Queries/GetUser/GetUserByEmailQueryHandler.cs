using MediatR;
using Model.DTO;
using Repositories.Utils;

namespace Domain.Queries.GetUser
{
    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, UserDTO>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByEmailQueryHandler(IUserRepository user_Repository)
        {
            _userRepository = user_Repository;
        }
        public async Task<UserDTO> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                throw new Exception ("Usuário não encontrado.");
            }
            return user;
        }
    }
}
