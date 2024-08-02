using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Model;
using Model.DTO;
using Repositories.Utils;
using Services;
using StackExchange.Redis;
using TesteOAuth2Basico.Repository;

namespace TesteOAuth2Basico.Controllers
{
    public class GoogleOAuthController : Controller
    {
        private readonly GoogleOauthClient _googleOauthClient;
        private readonly GoogleOAuthSettings _googleOAuthSettings;
        private readonly UserRepository _userRepository;

        public GoogleOAuthController(GoogleOauthClient googleOauthClient, IOptions<GoogleOAuthSettings> googleOAuthSettings, UserRepository userRepository)
        {
            _googleOauthClient = googleOauthClient;
            _googleOAuthSettings = googleOAuthSettings.Value;
            _userRepository = userRepository;
        }
        [HttpGet("validate-token")]
        public async Task<IActionResult> ValidateToken(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return BadRequest("Token de acesso inválido.");
            }
            try
            {
                var isValidToken = await _googleOauthClient.ValidateAccessTokenAsync(accessToken);

                if (isValidToken)
                {
                    return Ok("Token de acesso é válido.");
                }
                else
                {
                    return Unauthorized("Token de acesso não é válido.");
                }
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, "Erro de comunicação com o servidor de validação. Tente novamente mais tarde.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno na VALIDAÇÃO. Tente novamente mais tarde.");
            }
        }
       
        [HttpPost("insert-token-redis")]
        public async Task<IActionResult> InsertTokenRedis(TokenDTO tokenUser)
        {
            string redisString = "localhost:6379";
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisString);
            IDatabase redisDatabase = redis.GetDatabase();
            bool result = _userRepository.PostTokenRedis(tokenUser);
            if (result)
            {
                return Ok("Token de acesso inserido no Redis com sucesso.");
            }
            else
            {
                return BadRequest("Token de acesso já existe.");
            }
        }
    }
}

