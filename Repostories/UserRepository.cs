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
        //private readonly IDatabase _redisDatabase;
        //private readonly string connectionString;

        //public UserRepository(IConfiguration configuration, IDatabase redisDatabase)//construtor que recebe a instância do IConfiguration
        //{
        //    connectionString = configuration.GetConnectionString("DefaultConnection");//inicializa string de conexão
        //    _redisDatabase = redisDatabase;
        //}
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
                    string insertUserQuery = "INSERT INTO Users (Name, Email, ProfileImageUrl) VALUES (@Name, @Email, @ProfileImageUrl);";
                    object obj = new
                    {
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
                IDatabase redisDatabase = redis.GetDatabase();
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



        //public async Task<bool> RegisterUserFromGoogle(GoogleUserData googleUser)
        //{
        //    if (googleUser == null || string.IsNullOrWhiteSpace(googleUser.Email) ||
        //        string.IsNullOrWhiteSpace(googleUser.AccessTokenGoogle) ||
        //        string.IsNullOrWhiteSpace(googleUser.RefreshTokenGoogle))
        //    {
        //        throw new ArgumentException("Dados do usuário inválidos.");
        //    }
        //    try
        //    {
        //        using (var sqlConnection = new SqlConnection(connectionString))
        //        {
        //            await sqlConnection.OpenAsync();
        //            string checkUserExists = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
        //            int userExists = await sqlConnection.ExecuteScalarAsync<int>(checkUserExists, new { Email = googleUser.Email });
        //            if (userExists > 0)
        //            {
        //                return false;
        //            }
        //            string insertUser = "INSERT INTO Users (Name, Email, ProfileImageUrl) OUTPUT Inserted.Id VALUES (@Name, @Email, @ProfileImageUrl)";
        //            var parameters = new
        //            {
        //                @Name = googleUser.Name,
        //                @Email = googleUser.Email,
        //                @ProfileImageUrl = googleUser.ProfileImageUrl
        //            };
        //            int rowsAffected = await sqlConnection.ExecuteAsync(insertUser, parameters);
        //            if (rowsAffected <= 0)
        //            {
        //                Console.WriteLine("Usuário não inserido.");
        //                return false;
        //            }
        //        }
        //        var userTokens = new GoogleUserData
        //        {
        //            IdUser = googleUser.IdUser,
        //            AccessTokenGoogle = googleUser.AccessTokenGoogle,
        //            RefreshTokenGoogle = googleUser.RefreshTokenGoogle,
        //            AccessTokenGoogleExpiresIn = googleUser.AccessTokenGoogleExpiresIn
        //        };
        //        string redisKey = $"user:{googleUser.IdUser}:tokens";
        //        var tokenValue = Newtonsoft.Json.JsonConvert.SerializeObject(userTokens);
        //        bool redisResult = await _redisDatabase.StringSetAsync(redisKey, tokenValue);
        //        if (!redisResult)
        //        {
        //            Console.WriteLine("Erro ao inserir no Redis.");
        //            return false;
        //        }
        //        return true;
        //    }
        //    catch (SqlException ex)
        //    {
        //        Console.WriteLine($"Erro de SQL: {ex.Message}");
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Erro geral: {ex.Message}");
        //        return false;
        //    }
        //}
    }
}