
namespace Model
{
    public class GoogleOAuthSettings //Configurações de autenticação do Google
    {
        public string ClientId { get; set; } //identifica a aplicação
        public string ClientSecret { get; set; } //chave de autenticação
        public string TokenEndpoint { get; set; } //endereço de requisição de token
        public string ApiEndpoint { get; set; } //URL base para acessar API do Google
        public string RedirectUri { get; set; } //URL para onde o Google redireciona após autenticação
    }
}
