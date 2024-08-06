using Cache;
using Domain.Commands;
using MediatR;
using TesteOAuth2Basico.Repository;


namespace Domain.Handlers
{
    public class InsertTokenCommandHandler : IRequestHandler<InsertTokenCommand, bool>
    {
        private readonly UserRepository _userRepository;
        private readonly CacheRedisRepository _userCacheRedis;
        public InsertTokenCommandHandler(UserRepository userRepository, CacheRedisRepository userCacheRedis)
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
