using Dapper;
using Microsoft.Data.SqlClient;
using Model;
using Repostories.Utils;
using TesteOAuth2Basico.Model;

namespace Services
{
    public class GoogleAuthorizationService
    {
        //SENHA GOOGLE
        public async Task<bool> CreateUserFromGooglePassword(GoogleUserData googleUser)
        {
            if (googleUser == null || string.IsNullOrWhiteSpace(googleUser.Email))
            {
                throw new ArgumentException("Dados do usuário inválidos.");
            }
            //hash da senha do usuário 
            string salt;
            string hashedPassword = PasswordHash.CreateHash("defaultPassword", out salt);
            string strConn = "Data Source=127.0.0.1; Initial Catalog=DBTstOAUthBas; User Id=sa; Password=SqlServer2019!; TrustServerCertificate=Yes;";
            try
            {
                using (var connection = new SqlConnection(strConn))
                {
                    connection.Open();
                    string insertUser = "INSERT INTO Users (IdUser, Name, Email, Password, ProfileImageUrl) " +
                        "VALUES (@IdUser, @Name, @Email, @Password, @ProfileImageUrl)";

                    var parameters = new
                    {
                        IdUser = googleUser.IdUser,
                        Name = googleUser.Name,
                        Email = googleUser.Email,
                        Password = hashedPassword,
                        ProfileImageUrl = googleUser.ProfileImageUrl
                    };
                    int rowsAffected = connection.Execute(insertUser, parameters);
                    return rowsAffected > 0;
                }
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

        //TOKEN GOOGLE
        public async Task<bool> CreateUserFromGoogleToken(GoogleUserData googleUser)
        {
            string _connectionString = "Data Source=127.0.0.1; Initial Catalog=DBTstOAUthBas; User Id=sa; Password=SqlServer2019!; TrustServerCertificate=Yes;";
            if (googleUser == null || string.IsNullOrWhiteSpace(googleUser.Email) || string.IsNullOrWhiteSpace(googleUser.AccessTokenGoogle))
            {
                throw new ArgumentException("Dados do usuário inválidos.");
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string insertUser = "INSERT INTO Users (IdUser, Name, Email, ProfileImageUrl) " +
                        "VALUES (@IdUser, @Name, @Email, @ProfileImageUrl)";

                    var parameters = new
                    {
                        IdUser = googleUser.IdUser,
                        Name = googleUser.Name,
                        Email = googleUser.Email,
                        ProfileImageUrl = googleUser.ProfileImageUrl
                    };

                    int rowsAffected = await connection.ExecuteAsync(insertUser, parameters);
                    return rowsAffected > 0;
                }
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

