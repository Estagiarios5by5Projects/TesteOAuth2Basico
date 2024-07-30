using TesteOAuth2Basico.Model;

namespace Repositories.Utils
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<User> GetUserByIdAsync(int userId);
    }
}