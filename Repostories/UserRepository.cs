using Microsoft.Data.SqlClient;
using Model;
using Dapper;
using StackExchange.Redis;
using Model.DTO;
using Microsoft.SqlServer.Management.SqlParser.Metadata;

namespace TesteOAuth2Basico.Repository
{
    public class UserRepository
    {
        public bool PostUserSql(UserDTO googleUser)
        {
            if (googleUser == null || string.IsNullOrWhiteSpace(googleUser.Email))
            {
                throw new ArgumentException("Dados do usuário inválidos.");
            }
            try
            {
                string connectionString = "Data Source=127.0.0.1; Initial Catalog=DBTstOAUthBas; User Id=sa; Password=SqlServer2019!; TrustServerCertificate=Yes;";
                using (var sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    string checkUserExists = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
                    int userExists = sqlConnection.ExecuteScalar<int>(checkUserExists, new { Email = googleUser.Email });
                    if (userExists > 0)
                    {
                        return false;
                    }
                    string insertUserQuery = "INSERT INTO Users (IdUser, Name, Email, ProfileImageUrl) VALUES (@IdUser, @Name, @Email, @ProfileImageUrl);";
                    object obj = new
                    {
                        @IdUser = googleUser.IdUser,
                        @Name = googleUser.Name,
                        @Email = googleUser.Email,
                        @ProfileImageUrl = googleUser.ProfileImageUrl
                    };
                    return sqlConnection.Execute(insertUserQuery, obj) > 0;
                }
            }catch (SqlException ex)
            {
                Console.WriteLine($"Erro de SQL: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro geral: {ex.Message}");
                return false;
            }
            
        }
        public bool PostTokenRedis(TokenDTO googleToken)
        {
            if (googleToken == null || string.IsNullOrWhiteSpace(googleToken.AccessTokenGoogle))
            {
                throw new ArgumentException("Token de acesso inválido.");
            }
            try
            {
                var userTokens = new TokenDTO
                {
                    IdUserToken = googleToken.IdUserToken,
                    AccessTokenGoogle = googleToken.AccessTokenGoogle,
                    RefreshTokenGoogle = googleToken.RefreshTokenGoogle,
                    AccessTokenGoogleExpiresIn = googleToken.AccessTokenGoogleExpiresIn
                };
                string redisString = "localhost:6379";
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisString);
                StackExchange.Redis.IDatabase redisDatabase = redis.GetDatabase();
                string redisKey = $"user:{googleToken.IdUserToken}:tokens";
                var tokenValue = Newtonsoft.Json.JsonConvert.SerializeObject(googleToken);
                return redisDatabase.StringSet(redisKey, tokenValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro geral: {ex.Message}");
                return false;
            }
        }
    }
}