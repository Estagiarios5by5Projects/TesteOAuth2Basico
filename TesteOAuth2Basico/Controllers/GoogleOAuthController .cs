using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Model;
using Services;

namespace TesteOAuth2Basico.Controllers
{
    public class GoogleOAuthController : Controller
    {
        private readonly GoogleOauthClient _googleOauthClient;
        private readonly GoogleOAuthSettings _googleOAuthSettings;
        public GoogleOAuthController(GoogleOauthClient googleOauthClient, IOptions<GoogleOAuthSettings> googleOAuthSettings)
        {
            _googleOauthClient = googleOauthClient;
            _googleOAuthSettings = googleOAuthSettings.Value;
        }
        [HttpGet("auth/callback")]
        public async Task<IActionResult> AuthCallback(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest("Código de autorização inválido.");
            }
            try
            {
                var tokenResponse = await _googleOauthClient.GetAccessTokenAsync(code, _googleOAuthSettings.RedirectUri);
                if (tokenResponse == null || string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
                {
                    return Unauthorized("Não foi possível obter um token de acesso. O código pode ser inválido ou expirado.");
                }
                var apiResponse = await _googleOauthClient.CallApiAsync(tokenResponse.AccessToken);
                return Content(apiResponse);
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, "Erro de comunicação com o servidor de autenticação. Tente novamente mais tarde.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno. Tente novamente mais tarde.");
            }
        }
    }
}

