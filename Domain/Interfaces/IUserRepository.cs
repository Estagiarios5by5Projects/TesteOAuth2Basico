using Model;
using Model.DTO;

namespace Repositories.Utils
{
    public interface IUserRepository
    {
        Task<bool> AddUserAsync(UserDTO user);
        Task<UserDTO?> GetUserByIdAsync(string userId);
    }
}