using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO
{
    public class TokenDTO
    {
        public string IdUserToken { get; set; }
        public string AccessTokenGoogle { get; set; }
        public string RefreshTokenGoogle { get; set; }
        public string AccessTokenGoogleExpiresIn { get; set; }
    }
}
