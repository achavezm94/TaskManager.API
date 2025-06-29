using TaskManager.API.Models;

namespace TaskManager.API.Services
{
    public interface IAuthService
    {
        Task<string> Login(string email, string password);
        Task<User> Register(User user, string password);
    }
}
