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
            _clientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
            _clientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));
            _tokenEndpoint = tokenEndpoint ?? throw new ArgumentNullException(nameof(tokenEndpoint));
            _apiEndpoint = apiEndpoint ?? throw new ArgumentNullException(nameof(apiEndpoint));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            // Inicializa o LoggerFactory e cria um logger sem passar pelo construtor
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<GoogleOauthClient>();
        }

        public async Task<bool> ValidateAccessTokenAsync(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }
            var tokenInfo = AppSettings.OAuthDataSettings.TokenInfoEndpoint;
            try
            {
                _logger.LogInformation("Iniciando a validação do token de acesso: {AccessToken}", accessToken);

                // Enviar uma solicitação GET para validar o token
                var response = await _httpClient.GetAsync($"{tokenInfo}?access_token={accessToken}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Erro ao validar token de acesso. StatusCode: {StatusCode}", response.StatusCode);
                    return false;
                }

                var responseBody = await response.Content.ReadAsStringAsync();

                // Aqui você pode verificar o conteúdo da resposta para assegurar que o token é válido
                // Normalmente, se o token não for válido, o Google retornará um erro com uma descrição

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

                    Address = AppSettings.OAuthDataSettings.TokenEndpoint,//endereço do token
                    Code = authorizationCode,//código de autorização                 
                    ClientId = AppSettings.OAuthDataSettings.ClientId,//ID do cliente                    
                    ClientSecret = AppSettings.OAuthDataSettings.ClientSecret,//chave secreta do cliente
                    RedirectUri = redirectUri//URI de redirecionamento, após autenticação

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

