using MediatR;
using Model.DTO;
using Repositories.Utils;
using ServiceStack.Host;
using System.Net;
using System.Web.Mvc;

namespace Domain.Queries.GetUser
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDTO>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(IUserRepository user_Repository)
        {
            _userRepository = user_Repository;
        }
        public async Task<UserDTO> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.IdUser);
            if (user == null)
            {
                throw new Exception ("Usuário não encontrado.");
            }
            return user;
        }
    }
}
