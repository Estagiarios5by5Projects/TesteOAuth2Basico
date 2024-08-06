using MediatR;
using Model.DTO;

namespace Domain.Commands
{
    public class CreateUserCommand : IRequest<bool>
    {
        public UserDTO User { get; set; }
    }
}
