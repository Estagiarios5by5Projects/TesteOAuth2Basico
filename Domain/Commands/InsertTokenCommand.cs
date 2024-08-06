using Model.DTO;
using MediatR;


namespace Domain.Commands
{
    public class InsertTokenCommand: IRequest<bool>
    {
        public TokenDTO Token { get; set; }

    }
}
