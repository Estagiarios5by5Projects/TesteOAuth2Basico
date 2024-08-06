using MediatR;
using Model.DTO;

namespace Domain.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<bool>
    {
        public UserDTO User { get; set; }
    }
}
