using Model.DTO;

namespace Domain.Interfaces
{
    public interface ICacheRedisRepository
    {
        bool PostTokenRedis(TokenDTO googleToken);

    }
}
