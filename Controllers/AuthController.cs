using Microsoft.AspNetCore.Mvc;
using TaskManager.API.DTOs.Auth;
using TaskManager.API.Models;
using TaskManager.API.Services;

namespace TaskManager.API.Controllers
{
    /// <summary>
    /// Controlador para autenticación de usuarios (registro y login).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="dto">Datos del usuario a registrar.</param>
        /// <returns>Mensaje de éxito si el registro fue exitoso.</returns>
        /// <response code="200">Usuario registrado exitosamente.</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Role = dto.Role
            };
            _ = await _authService.Register(user, dto.Password);
            return Ok(new { Message = "Usuario registrado exitosamente" });
        }

        /// <summary>
        /// Autentica a un usuario y devuelve un token JWT.
        /// </summary>
        /// <param name="dto">Credenciales de acceso del usuario.</param>
        /// <returns>Token JWT si la autenticación es exitosa.</returns>
        /// <response code="200">Autenticación exitosa, token devuelto.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _authService.Login(dto.Email, dto.Password);
            return Ok(new AuthResponseDto { Token = token });
        }
    }
}
