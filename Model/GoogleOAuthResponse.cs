﻿
namespace Model
{
    public class GoogleOAuthResponse
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public DateTime expires_in { get; set; }
       
    }
}
