using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManager.API.Data;
using TaskManager.API.Models;

namespace TaskManager.API.Services
{
    public class AuthService(
        AppDbContext context,
        IConfiguration config,
        IPasswordHasher<User> passwordHasher) : IAuthService
    {
        private readonly AppDbContext _context = context;
        private readonly IConfiguration _config = config;
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

        public async Task<string> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email) ?? throw new Exception("Usuario no encontrado");
            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password
            );

            if (result != PasswordVerificationResult.Success)
                throw new Exception("Credenciales inválidas");

            return GenerateJwtToken(user);
        }

        public async Task<User> Register(User user, string password)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                throw new Exception("Email ya registrado");

            user.PasswordHash = _passwordHasher.HashPassword(user, password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}