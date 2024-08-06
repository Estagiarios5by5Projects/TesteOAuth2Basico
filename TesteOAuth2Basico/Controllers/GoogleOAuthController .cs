using Domain.Commands;
using Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;

namespace TesteOAuth2Basico.Controllers
{
    public class GoogleOAuthController : Controller
    {
        private readonly IMediator _mediator;
        public GoogleOAuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("validate-token")]
        public async Task<IActionResult> ValidateToken(string accessToken)
        {
            var query = new ValidateTokenQuery { AccessToken = accessToken };
            var isValidToken = await _mediator.Send(query);
            if (isValidToken)
            {
                return Ok("Token de acesso é válido.");
            }
            else
            {
                return Unauthorized("Token de acesso não é válido.");
            }
        }

        [HttpPost("insert-token-redis")]
        public async Task<IActionResult> InsertTokenRedis(TokenDTO tokenUser)
        {
            var command = new InsertTokenCommand { Token = tokenUser };
            var token = await _mediator.Send(command);
            if (token)
            {
                return Ok("Token de acesso inserido no Redis com sucesso.");
            }
            else
            {
                return BadRequest("Token de acesso já existe.");
            }
        }
        //public async Task<IActionResult> InsertTokenRedis(TokenDTO tokenUser)
        //{
        //    string redisString = "localhost:6379";
        //    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisString);
        //    IDatabase redisDatabase = redis.GetDatabase();
        //    bool result = await _mediator.PostTokenRedis(tokenUser); // Alteração: Adicionar o await para aguardar a conclusão do método assíncrono
        //    if (result)
        //    {
        //        return Ok("Token de acesso inserido no Redis com sucesso.");
        //    }
        //    else
        //    {
        //        return BadRequest("Token de acesso já existe.");
        //    }
        //}
        //public async Task<IActionResult> ValidateToken(string accessToken)
        //{
        //    if (string.IsNullOrWhiteSpace(accessToken))
        //    {
        //        return BadRequest("Token de acesso inválido.");
        //    }
        //    try
        //    {
        //        var isValidToken = await _googleOauthClient.ValidateAccessTokenAsync(accessToken);

        //        if (isValidToken)
        //        {
        //            return Ok("Token de acesso é válido.");
        //        }
        //        else
        //        {
        //            return Unauthorized("Token de acesso não é válido.");
        //        }
        //    }
        //    catch (HttpRequestException)
        //    {
        //        return StatusCode(503, "Erro de comunicação com o servidor de validação. Tente novamente mais tarde.");
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, "Ocorreu um erro interno na VALIDAÇÃO. Tente novamente mais tarde.");
        //    }
        //}

        //public async Task<IActionResult> InsertTokenRedis(TokenDTO tokenUser)
        //{ var command = new InsertTokenCommand { Token = tokenUser };
        //    var token = await _mediator.Send(command);
        //    if (token)
        //    {
        //        return Ok("Token de acesso inserido no Redis com sucesso.");
        //    }
        //    else
        //    {
        //        return BadRequest("Token de acesso já existe.");
        // }
        //public async Task<IActionResult> InsertTokenRedis(TokenDTO tokenUser)
        //{
        //    string redisString = "localhost:6379";
        //    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisString);
        //    IDatabase redisDatabase = redis.GetDatabase();
        //    bool result = _userRepository.PostTokenRedis(tokenUser);
        //    if (result)
        //    {
        //        return Ok("Token de acesso inserido no Redis com sucesso.");
        //    }
        //    else
        //    {
        //        return BadRequest("Token de acesso já existe.");
        //    }
        //}
    }
}

