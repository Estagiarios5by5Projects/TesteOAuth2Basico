using Domain.Interfaces;
using MediatR;
using Repositories.Utils;


namespace Domain.Commands.InsertToken
{
    public class InsertTokenCommandHandler : IRequestHandler<InsertTokenCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICacheRedisRepository _userCacheRedis;
        public InsertTokenCommandHandler(IUserRepository userRepository, ICacheRedisRepository userCacheRedis)
        {
            _userRepository = userRepository;
            _userCacheRedis = userCacheRedis;
        }

        public async Task<bool> Handle(InsertTokenCommand request, CancellationToken cancellationToken)
        {
            return _userCacheRedis.PostTokenRedis(request.Token);

        }
    }
}
