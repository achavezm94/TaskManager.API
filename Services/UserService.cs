using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.API.Data;
using TaskManager.API.Models;

namespace TaskManager.API.Services
{
    public class UserService(
        AppDbContext context,
        IPasswordHasher<User> passwordHasher) : IUserService
    {
        private readonly AppDbContext _context = context;
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<int> GetUserCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id)
                ?? throw new Exception("Usuario no encontrado");
        }

        public async Task<User> CreateUser(User user, string password)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                throw new Exception("Email ya registrado");

            user.PasswordHash = _passwordHasher.HashPassword(user, password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateUser(int id, User updatedUser)
        {
            var user = await _context.Users.FindAsync(id)
                ?? throw new Exception("Usuario no encontrado");

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.Role = updatedUser.Role;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}