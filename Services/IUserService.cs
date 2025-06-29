using TaskManager.API.Models;

namespace TaskManager.API.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsers();
        Task<int> GetUserCountAsync();
        Task<User> GetUserById(int id);
        Task<User> CreateUser(User user, string password);
        Task UpdateUser(int id, User user);
        Task DeleteUser(int id);
    }
}
