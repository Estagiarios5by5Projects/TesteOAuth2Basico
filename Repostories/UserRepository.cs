using Microsoft.Data.SqlClient;
using MongoDB.Driver;
using TesteOAuth2Basico.Model;
using Dapper;
using Azure.Core;
using Model;
using StackExchange.Redis;
using Babel.ParserGenerator;

namespace TesteOAuth2Basico.Repository
{
    public class UserRepository
    {
        private readonly IDatabase _redisDatabase;
        //private readonly IMongoCollection<GoogleUserToken> _tokensCollection;
        private readonly string _sqlConnectionString = "Data Source=127.0.0.1; Initial Catalog=DBTstOAUthBas; User Id=sa; Password=SqlServer2019!; TrustServerCertificate=Yes;";
        //var mongoClient = new MongoClient("mongodb+srv://<username>:<password>@cluster0.mongodb.net/test?retryWrites=true&w=majority");
        //var mongoDatabase = mongoClient.GetDatabase("YourDatabaseName");
        //_tokensCollection = mongoDatabase.GetCollection<GoogleUserToken>("UserToken");
        public UserRepository(IDatabase redisDatabase)
        {
            _redisDatabase = redisDatabase;
        }

        public async Task<bool> RegisterUserFromGoogle(GoogleUserData googleUser)
        {
            if (googleUser == null || string.IsNullOrWhiteSpace(googleUser.Email) ||
                string.IsNullOrWhiteSpace(googleUser.AccessTokenGoogle) ||
                string.IsNullOrWhiteSpace(googleUser.RefreshTokenGoogle))
            {
                throw new ArgumentException("Dados do usuário inválidos.");
            }

            int userId;
            try
            {
                using (var sqlConnection = new SqlConnection(_sqlConnectionString))
                {
                    await sqlConnection.OpenAsync();

                    string insertUser = "INSERT INTO Users (Name, Email, ProfileImageUrl) OUTPUT Inserted.Id VALUES (@Name, @Email, @ProfileImageUrl)";
                    var parameters = new
                    {
                        Name = googleUser.Name,
                        Email = googleUser.Email,
                        ProfileImageUrl = googleUser.ProfileImageUrl
                    };
                    userId = await sqlConnection.ExecuteScalarAsync<int>(insertUser, parameters);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir no SQL Server: {ex.Message}");
                return false;
            }

            try
            {
                var userTokens = new GoogleUserData
                {
                    IdUser = googleUser.IdUser,
                    AccessTokenGoogle = googleUser.AccessTokenGoogle,
                    RefreshTokenGoogle = googleUser.RefreshTokenGoogle,
                    AccessTokenGoogleExpiresIn = googleUser.AccessTokenGoogleExpiresIn
                };

                string redisKey = $"user:{googleUser.IdUser}:tokens";
                var tokenValue = Newtonsoft.Json.JsonConvert.SerializeObject(userTokens);

                await _redisDatabase.StringSetAsync(redisKey, tokenValue);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir no Redis: {ex.Message}");
                return false;
            }

            //    await _tokensCollection.InsertOneAsync(userTokens);
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Erro ao inserir no MongoDB: {ex.Message}");
            //    return false;
            //}
        }
    }
}
