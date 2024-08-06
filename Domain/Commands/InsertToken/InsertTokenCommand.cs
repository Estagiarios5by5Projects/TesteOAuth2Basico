using Model.DTO;
using MediatR;


namespace Domain.Commands.InsertToken
{
    public class InsertTokenCommand : IRequest<bool>
    {
        public TokenDTO Token { get; set; }

    }
}
