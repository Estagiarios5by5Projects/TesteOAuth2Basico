using CrossCutting.Configuration;
using Dapper;
using Microsoft.Data.SqlClient;
using Model.DTO;
using Repositories.Utils;

namespace TesteOAuth2Basico.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly string? _connectionString;
        public UserRepository()
        {
            _connectionString = AppSettings.SqlDataSettings.ConnectionString;

        }

        public async Task<bool> AddUserAsync(UserDTO googleUser)
        {
            if (googleUser == null || string.IsNullOrWhiteSpace(googleUser.Email))
            {
                throw new ArgumentException("Dados do usuário inválidos.");
            }
            try
            {

                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    string checkUserExists = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
                    int userExists = sqlConnection.ExecuteScalar<int>(checkUserExists, new { Email = googleUser.Email });
                    if (userExists > 0) //retorna 0 se não existir e 1 se existir
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
                    var response = await sqlConnection.ExecuteAsync(insertUserQuery, obj);
                    return true;
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Erro de SQL: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro geral: {ex.Message}");
                throw;
            }

        }
        public async Task<UserDTO?> GetUserByEmailAsync(string email)
        {

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                string query = "SELECT IdUser, Name, Email, ProfileImageUrl FROM Users WHERE Email = @Email";
                var response = await sqlConnection.QueryAsync<UserDTO>(query, new { Email = email });
                return response.FirstOrDefault();
            }
        }
    }
}