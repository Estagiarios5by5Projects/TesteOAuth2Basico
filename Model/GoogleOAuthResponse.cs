
namespace Model
{
    public class GoogleOAuthResponse //Resposta de autenticação do Google
    {
        public string AccessToken { get; set; } //token de acesso
        public string RefreshToken { get; set; } //token de atualização, usado para obter novo token de acesso
        public DateTime ExpiresIn { get; set; } //tempo de expiração do token de acesso
       
    }
}
