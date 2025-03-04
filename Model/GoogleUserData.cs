﻿
namespace Model
{
    public class GoogleUserData //Dados do usuário autenticado pelo Google
    {
        public string IdUser { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ProfileImageUrl { get; set; }
        public string AccessTokenGoogle { get; set; }
        public string RefreshTokenGoogle { get; set; }
        public DateTime AccessTokenGoogleExpiresIn { get; set; }
    }
}
