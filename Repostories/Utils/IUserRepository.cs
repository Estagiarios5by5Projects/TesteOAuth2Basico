using Model;

namespace Repositories.Utils
{
    public interface IUserRepository
    {
        Task AddUserAsync(GoogleUserData user);
        Task<GoogleUserData> GetUserByIdAsync(string userId);
    }
}