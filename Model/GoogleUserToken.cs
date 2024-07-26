using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TesteOAuth2Basico.Model
{
    public class GoogleUserToken
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string IdUser { get; set; }
        public string AccessTokenGoogle { get; set; }
        public string RefreshTokenGoogle { get; set; }
        public DateTime AccessTokenGoogleExpiresIn { get; set; }
    }
}
