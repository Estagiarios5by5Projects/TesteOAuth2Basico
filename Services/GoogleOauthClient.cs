using Amazon.Runtime.Internal.Util;
using Azure;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.SqlParser.Parser;
using Model;
using System.Net.Http;

namespace Services
{
    public class GoogleOauthClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _tokenEndpoint;
        private readonly string _apiEndpoint;

        public GoogleOauthClient(string clientId, string clientSecret, string tokenEndpoint, string apiEndpoint)//construtor, recebe dependências
        {
            //inicializa, garantindo que não seja nulo
            _clientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
            _clientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));
            _tokenEndpoint = tokenEndpoint ?? throw new ArgumentNullException(nameof(tokenEndpoint));
            _apiEndpoint = apiEndpoint ?? throw new ArgumentNullException(nameof(apiEndpoint));
        }
        public async Task<GoogleOAuthResponse> GetAccessTokenAsync(string authorizationCode, string redirectUri) //obtem token de acesso usando um código de autorização e URI de redirecionamento
        {
            if (string.IsNullOrEmpty(authorizationCode))//código de autorização != nulo
            {
                throw new ArgumentNullException(nameof(authorizationCode));
            }
            if (string.IsNullOrEmpty(redirectUri))//URI de redirecionamento != nulo
            {
                throw new ArgumentNullException(nameof(redirectUri));
            }
            try
            {   //faz requisição para obter token de acesso
                var tokenResponse = await _httpClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
                {
                    Address = _tokenEndpoint,//endpoint de troca de código por token
                    Code = authorizationCode,//código de autorização
                    ClientId = _clientId,//ID do cliente
                    ClientSecret = _clientSecret,//chave secreta do cliente
                    RedirectUri = redirectUri//URI de redirecionamento, após autenticação
                });
                if (tokenResponse.IsError)//erro de resposta do token
                {
                    
                    throw new Exception($"Erro ao obter token de acesso: {tokenResponse.Error}");//lança exceção com mensagem   
                }
                return new GoogleOAuthResponse //retorna objeto com token de acesso
                {
                    AccessToken = tokenResponse.AccessToken,//token de acesso
                    RefreshToken = tokenResponse.RefreshToken,//token de atualização
                    ExpiresIn = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn)//tempo de expiração
                };
            }
            catch (Exception ex)
            {
                
                throw;//lança exceção
            }
        }
        public async Task<string> CallApiAsync(string accessToken)//chama a API usando token de acesso
        {
            if (string.IsNullOrEmpty(accessToken))//token de acesso != nulo
            {
                throw new ArgumentNullException(nameof(accessToken));
            }
            try
            {
                _httpClient.SetBearerToken(accessToken);//define token de acesso no cabeçalho da requisição HTTP
                var response = await _httpClient.GetAsync(_apiEndpoint);//faz requisição Get para o Endpoint da API
                if (!response.IsSuccessStatusCode)//verifica resposta da API
                {
                   
                    throw new Exception($"Erro ao chamar a API: {response.StatusCode}");
                }
                return await response.Content.ReadAsStringAsync();//lê a resposta da API
            }
            catch
            {
                throw new Exception();
            }
        }
    }   
}


