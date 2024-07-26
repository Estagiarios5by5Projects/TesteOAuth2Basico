using IdentityModel.Client;
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
            public GoogleOauthClient(string clientId, string clientSecret, string tokenEndpoint, string apiEndpoint)
            {
                _httpClient = new HttpClient();
                _clientId = clientId;
                _clientSecret = clientSecret;
                _tokenEndpoint = tokenEndpoint;
                _apiEndpoint = apiEndpoint;
            }
            public async Task<GoogleOAuthResponse> GetAccessTokenAsync(string authorizationCode, string redirectUri)
            {
                var tokenResponse = await _httpClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
                {
                    Address = _tokenEndpoint,
                    Code = authorizationCode,
                    ClientId = _clientId,
                    ClientSecret = _clientSecret,
                    RedirectUri = redirectUri
                });
                if (tokenResponse.IsError)
                {
                    throw new Exception($"Erro ao obter token de acesso: {tokenResponse.Error}");
                }
                return new GoogleOAuthResponse
                {
                    access_token = tokenResponse.AccessToken,
                    refresh_token = tokenResponse.RefreshToken,
                    expires_in = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
                };
            }
            public async Task<string> CallApiAsync(string accessToken)
            {
                _httpClient.SetBearerToken(accessToken);
                var response = await _httpClient.GetAsync(_apiEndpoint);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Erro ao chamar a API: {response.StatusCode}");
                }
                return await response.Content.ReadAsStringAsync();
            }
        }
    }


