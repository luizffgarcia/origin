using OriginDatabase.Models;

namespace OriginUserAccessManagementService.Interfaces
{
    public interface IUserServiceHttpClient
    {
        Task<User> GetUserByEmailAsync(string email, bool isEmployerAdded);
        Task<bool> CreateUserAsync(User user);
    }
}
