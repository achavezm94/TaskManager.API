using TaskManager.API.Models;

namespace TaskManager.API.DTOs.Users
{
    public class UserUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.Employee;
    }
}
