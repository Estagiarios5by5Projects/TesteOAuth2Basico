using Cache;
using CrossCutting.Configuration;
using Model.DTO;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repostories
{
    public class CacheRedisRepository
    {
        private readonly string? _connectionStringRedis;
        public CacheRedisRepository()
        {
            _connectionStringRedis = CacheSettings.RedisDataSettings.ConnectionStringRedis;

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
