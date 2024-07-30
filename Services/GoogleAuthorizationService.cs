using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model;
using System;
using System.Threading.Tasks;


namespace Services
{
    public class GoogleAuthorizationService //inserir informação do usuário autenticado no banco de dados
    {
        private readonly string connectionString;
        public GoogleAuthorizationService(IConfiguration configuration)//construtor que recebe a instância do IConfiguration
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");//inicializa string de conexão
        }
        //TOKEN GOOGLE
        public async Task<bool> CreateUserFromGoogleToken(GoogleUserData googleUser)//cria o usuário no banco de dados a partir do token do google
        {
            //validação de dados, se o usuário é nulo, se o email é nulo ou vazio, se o token de acesso do google é nulo ou vazio
            if (googleUser == null || string.IsNullOrWhiteSpace(googleUser.Email) || string.IsNullOrWhiteSpace(googleUser.AccessTokenGoogle))
            {
                throw new ArgumentException("Dados do usuário inválidos.");
            }
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();//abre conexão de forma assíncrona
                    string checkUserExists = "SELECT COUNT(1) FROM Users WHERE Email = @Email";//verifica se já existe usuário com o mesmo email
                    bool userExists = await connection.ExecuteScalarAsync<bool>(checkUserExists, new { Email = googleUser.Email });//executa a consulta SQL para verificar a existência do usuário
                    if (userExists)
                    {
                        return false;//retorna false se o usuário já existir
                    }
                    //inserir novo usuário no banco de dados
                    string insertUser = "INSERT INTO Users (IdUser, Name, Email, ProfileImageUrl) " +
                        "VALUES (@IdUser, @Name, @Email, @ProfileImageUrl)";
                    //cria um objeto com os parâmetros para a inserção
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
            catch (SqlException ex) //erros do SQL
            {
                Console.WriteLine($"Erro de SQL: {ex.Message}");
                return false;
            }
            catch (Exception ex)//erros gerais
            {
                Console.WriteLine($"Erro geral: {ex.Message}");
                return false;
            }
        }
    }
}
