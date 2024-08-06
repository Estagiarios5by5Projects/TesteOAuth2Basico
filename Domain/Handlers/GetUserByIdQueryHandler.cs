using Domain.Queries;
using MediatR;
using Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteOAuth2Basico.Repository;

namespace Domain.Handlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDTO>
    {
        private readonly UserRepository _userRepository;

        public GetUserByIdQueryHandler(UserRepository user_Repository)
        {
            _userRepository = user_Repository;
        }
        public async Task<UserDTO> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.IdUser);
            if (user == null)
            {
                throw new ArgumentException("Usuário não encontrado.");
            }
            return user;
        }
    }
}
