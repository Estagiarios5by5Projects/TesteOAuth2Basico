using Model;
using Newtonsoft.Json;
using StackExchange.Redis;
namespace Services
{
    public class RedisCache
    {
        private readonly IConnectionMultiplexer _redis;//instância o IConnectionMultiplexer, que gerencia a conexão com o servidor Redis
        public RedisCache(IConnectionMultiplexer redis)//Construtor: recebe o IConnectionMultiplexer por injeção de dependência.
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));//garante que não seja nulo
        }
        public async Task SetStringAsync(string key, string value)//armazena string com chave específica no Redis
        {
            var db = _redis.GetDatabase();//obtem instância no Redis
            await db.StringSetAsync(key, value);//armazena o valor associado na chave
        }
        public async Task<string> GetStringAsync(string key)//recupera do Redis
        {
            var db = _redis.GetDatabase();
            return await db.StringGetAsync(key);//retorna valor e se não existir, retorna null
        }
        //public async Task CacheOAuthResponseAsync(string userId, GoogleOAuthResponse oAuthResponse) //receber resposta da autenticação e setar no Redis
        //{
        //    var db = _redis.GetDatabase();
        //    var json = JsonSerializer.Serialize(oAuthResponse);
        //    await _redis.StringSetAsync                     
        //}

        //public async Task<GoogleOAuthResponse> RetrieveOAuthResponseAsync(string userId)//Retornar resposta de autenticação salva no Redis
        //{
        //    var db = _redis.GetDatabase();
        //    return <GoogleOAuthResponse>
        //}
    }
}
