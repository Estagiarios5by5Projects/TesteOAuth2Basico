using Microsoft.AspNetCore.Mvc;
using Model;
using Services;

namespace TesteOAuth2Basico.Controllers
{
    public class GoogleOAuthController : Controller
    {
        private readonly GoogleOauthClient _googleOauthClient;
        public GoogleOAuthController(GoogleOauthClient googleOauthClient)
        {
            _googleOauthClient = googleOauthClient;
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
                var settings = new GoogleOAuthSettings
            {
                RedirectUri = "https://auth.expo.io/@luanlf/AuthenticatorTreining"
            };
                var tokenResponse = await _googleOauthClient.GetAccessTokenAsync(code, settings.RedirectUri);
                if (tokenResponse == null || string.IsNullOrWhiteSpace(tokenResponse.access_token))
                {
                    return Unauthorized("Não foi possível obter um token de acesso. O código pode ser inválido ou expirado.");
                }

                var apiResponse = await _googleOauthClient.CallApiAsync(tokenResponse.access_token);
                return Content(apiResponse);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, "Erro de comunicação com o servidor de autenticação. Tente novamente mais tarde.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocorreu um erro interno. Tente novamente mais tarde.");
            }
        }
    }
}
