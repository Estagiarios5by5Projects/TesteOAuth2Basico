using CrossCutting.Configuration;
using Domain.Models;
using StackExchange.Redis;

namespace Cache
{

    public class CacheRedisRepository
    {
        private readonly string? _connectionStringRedis;
        public CacheRedisRepository()
        {
            _connectionStringRedis = AppSettings.RedisDataSettings.ConnectionStringRedis;

        }

        public bool PostTokenRedis(TokenDTO googleToken)
        {
            if (googleToken == null || string.IsNullOrWhiteSpace(googleToken.AccessTokenGoogle))
            {
                throw new ArgumentException("Token de acesso inválido.");
            }
            try
            {
                var userTokens = new TokenDTO
                {
                    IdUserToken = googleToken.IdUserToken,
                    AccessTokenGoogle = googleToken.AccessTokenGoogle,
                    RefreshTokenGoogle = googleToken.RefreshTokenGoogle,
                    AccessTokenGoogleExpiresIn = googleToken.AccessTokenGoogleExpiresIn
                };

                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(_connectionStringRedis);
                StackExchange.Redis.IDatabase redisDatabase = redis.GetDatabase();
                string redisKey = $"user:{googleToken.IdUserToken}:tokens";
                var tokenValue = Newtonsoft.Json.JsonConvert.SerializeObject(googleToken);
                return redisDatabase.StringSet(redisKey, tokenValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro geral: {ex.Message}");
                return false;
            }
        }
    }
}
