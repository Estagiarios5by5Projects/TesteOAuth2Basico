using Domain.Queries;
using MediatR;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Handlers
{
    public class ValidateTokenQueryHandler : IRequestHandler<ValidateTokenQuery, bool>
    {
        private readonly GoogleOauthClient _googleOauthClient;

        public ValidateTokenQueryHandler(GoogleOauthClient googleOauthClient)
        {
            _googleOauthClient = googleOauthClient;
        }

        public async Task<bool> Handle(ValidateTokenQuery request, CancellationToken cancellationToken)
        {
            return await _googleOauthClient.ValidateAccessTokenAsync(request.AccessToken);
        }

    }
}
