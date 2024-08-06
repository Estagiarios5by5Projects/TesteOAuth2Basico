using MediatR;
using Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Queries
{
    public class GetUserByIdQuery : IRequest<UserDTO>
    {
        public string IdUser { get; set; }
    }
}
