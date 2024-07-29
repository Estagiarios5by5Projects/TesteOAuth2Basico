namespace TesteOAuth2Basico.Model
{
    public class User
    {
        public string IdUser { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ProfileImageUrl { get; set; }
        public string AccessTokenGoogle { get; set; } 
        public string RefreshTokenGoogle { get; set; } 
        public DateTime AccessTokenGoogleExpiresIn { get; set; } 
    }
}
