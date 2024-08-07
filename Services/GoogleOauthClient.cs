using CrossCutting.Configuration;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Model;


namespace Services
{
    public class GoogleOauthClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _tokenEndpoint;
        private readonly string _apiEndpoint;
        private readonly ILogger<GoogleOauthClient> _logger;

        public GoogleOauthClient(
            string clientId,
            string clientSecret,
            string tokenEndpoint,
            string apiEndpoint,
            HttpClient httpClient
        )
        {
            _clientId = AppSettings.OAuthDataSettings.ClientId;
            _clientSecret = AppSettings.OAuthDataSettings.ClientSecret;
            _tokenEndpoint = AppSettings.OAuthDataSettings.TokenEndpoint;
            _apiEndpoint = AppSettings.OAuthDataSettings.ApiEndpoint;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<GoogleOauthClient>();
        }

        public async Task<bool> ValidateAccessTokenAsync(string accessToken)
        {
            var tokenInfo = AppSettings.OAuthDataSettings.TokenInfoEndpoint;
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }       
            try
            {
                _logger.LogInformation("Iniciando a validação do token de acesso: {AccessToken}", accessToken);

                
                var response = await _httpClient.GetAsync($"{tokenInfo}{accessToken}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Erro ao validar token de acesso. StatusCode: {StatusCode}", response.StatusCode);
                    return false;
                }

                var responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Token de acesso validado com sucesso.");
                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao validar o token de acesso do Google.");
                return false;
            }
        }

        public async Task<GoogleOAuthResponse> GetAccessTokenAsync(string authorizationCode, string redirectUri)
        {
            if (string.IsNullOrEmpty(authorizationCode))
            {
                throw new ArgumentNullException(nameof(authorizationCode));
            }
            if (string.IsNullOrEmpty(redirectUri))
            {
                throw new ArgumentNullException(nameof(redirectUri));
            }
            
            try
            {
                _logger.LogInformation("Iniciando a solicitação de token de acesso com o código de autorização: {AuthorizationCode} e redirectUri: {RedirectUri}", authorizationCode, redirectUri);

                var tokenResponse = await _httpClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
                {

                    Address = AppSettings.OAuthDataSettings.TokenEndpoint,
                    Code = authorizationCode,                
                    ClientId = AppSettings.OAuthDataSettings.ClientId,                  
                    ClientSecret = AppSettings.OAuthDataSettings.ClientSecret,
                    RedirectUri = redirectUri

                });

                if (tokenResponse.IsError)
                {
                    _logger.LogError("Erro ao obter token de acesso. Erro: {Error}, Descrição do Erro: {ErrorDescription}", tokenResponse.Error, tokenResponse.ErrorDescription);
                    throw new Exception($"Erro ao obter token de acesso: {tokenResponse.Error}");
                }

                _logger.LogInformation("Token de acesso obtido com sucesso. AccessToken: {AccessToken}, RefreshToken: {RefreshToken}, ExpiresIn: {ExpiresIn}",
                    tokenResponse.AccessToken, tokenResponse.RefreshToken, tokenResponse.ExpiresIn);

                return new GoogleOAuthResponse
                {
                    AccessToken = tokenResponse.AccessToken,
                    RefreshToken = tokenResponse.RefreshToken,
                    ExpiresIn = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao obter o token de acesso do Google.");
                throw new Exception("Falha ao obter o token de acesso do Google.", ex);
            }
        }

        public async Task<string> CallApiAsync(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            try
            {
                _logger.LogInformation("Iniciando a chamada à API com AccessToken: {AccessToken}", accessToken);

                _httpClient.SetBearerToken(accessToken);
                var response = await _httpClient.GetAsync(_apiEndpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Erro ao chamar a API. StatusCode: {StatusCode}", response.StatusCode);
                    throw new Exception($"Erro ao chamar a API: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Resposta da API recebida com sucesso. Conteúdo: {Content}", content);

                return content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao chamar a API.");
                throw new Exception("Falha ao chamar a API.", ex);
            }
        }
    }
}

