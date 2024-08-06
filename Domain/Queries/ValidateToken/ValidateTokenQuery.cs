using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Queries.ValidateToken
{
    public class ValidateTokenQuery : IRequest<bool>
    {
        public string AccessToken { get; set; }

    }
}
