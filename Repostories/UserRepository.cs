using Microsoft.Data.SqlClient;
using Model;
using Dapper;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TesteOAuth2Basico.Repository
{
    public class UserRepository
    {
        private readonly IDatabase _redisDatabase;
        private readonly string connectionString;

        public UserRepository(IConfiguration configuration, IDatabase redisDatabase)//construtor que recebe a instância do IConfiguration
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");//inicializa string de conexão
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
            try
            {
                using (var sqlConnection = new SqlConnection(connectionString))
                {
                    await sqlConnection.OpenAsync();
                    string checkUserExists = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
                    int userExists = await sqlConnection.ExecuteScalarAsync<int>(checkUserExists, new { Email = googleUser.Email });
                    if (userExists > 0)
                    {
                        return false;
                    }
                    string insertUser = "INSERT INTO Users (Name, Email, ProfileImageUrl) OUTPUT Inserted.Id VALUES (@Name, @Email, @ProfileImageUrl)";
                    var parameters = new
                    {
                        Name = googleUser.Name,
                        Email = googleUser.Email,
                        ProfileImageUrl = googleUser.ProfileImageUrl
                    };
                    int rowsAffected = await sqlConnection.ExecuteAsync(insertUser, parameters);
                    if (rowsAffected <= 0)
                    {
                        Console.WriteLine("Usuário não inserido.");
                        return false;
                    }
                }
                var userTokens = new GoogleUserData
                {
                    IdUser = googleUser.IdUser,
                    AccessTokenGoogle = googleUser.AccessTokenGoogle,
                    RefreshTokenGoogle = googleUser.RefreshTokenGoogle,
                    AccessTokenGoogleExpiresIn = googleUser.AccessTokenGoogleExpiresIn
                };
                string redisKey = $"user:{googleUser.IdUser}:tokens";
                var tokenValue = Newtonsoft.Json.JsonConvert.SerializeObject(userTokens);
                bool redisResult = await _redisDatabase.StringSetAsync(redisKey, tokenValue);
                if (!redisResult)
                {
                    Console.WriteLine("Erro ao inserir no Redis.");
                    return false;
                }
                return true;
            }
            catch (SqlException ex)
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
    }
}