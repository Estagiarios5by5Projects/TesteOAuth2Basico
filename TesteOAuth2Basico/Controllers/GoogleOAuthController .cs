using Microsoft.AspNetCore.Mvc;
using Model;
using Services;

namespace TesteOAuth2Basico.Controllers
{
    public class GoogleOAuthController : Controller
    {
        private readonly GoogleOauthClient _googleOAuthClient;
        public GoogleOAuthController(GoogleOauthClient googleOAuthClient)
        {
            _googleOAuthClient = googleOAuthClient;
        }
        [HttpGet("auth/callback")]
        public async Task<IActionResult> AuthCallback(string code)
        {
            var settings = new GoogleOAuthSettings
            {
                RedirectUri = "REDIRECT_URI" 
                //Na configuração do "ID do cliente OAuth",
                //URI de redirecionamento autorizado: "https://seusite.com/auth/callback" 
            };
            var tokenResponse = await _googleOAuthClient.GetAccessTokenAsync(code, settings.RedirectUri);
            var apiResponse = await _googleOAuthClient.CallApiAsync(tokenResponse.access_token);
            return Content(apiResponse);
        }
    }
}
