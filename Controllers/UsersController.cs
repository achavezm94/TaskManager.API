using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.API.DTOs.Users;
using TaskManager.API.Models;
using TaskManager.API.Services;

namespace TaskManager.API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de usuarios.
    /// Solo accesible para administradores.
    /// </summary>    
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Obtiene la lista de todos los usuarios.
        /// </summary>
        /// <returns>Lista de usuarios.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users.Select(u => new UserResponseDto(u)));
        }

        /// <summary>
        /// Devuelve la cantidad total de usuarios.
        /// </summary>
        /// <returns>Número total de usuarios.</returns>
        [HttpGet("count")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCount()
        {
            var count = await _userService.GetUserCountAsync();
            return Ok(count);
        }

        /// <summary>
        /// Obtiene un usuario por su identificador.
        /// </summary>
        /// <param name="id">Identificador del usuario.</param>
        /// <returns>Usuario encontrado.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetUserById(id);
            return Ok(new UserResponseDto(user));
        }

        /// <summary>
        /// Crea un nuevo usuario.
        /// </summary>
        /// <param name="dto">Datos del usuario a crear.</param>
        /// <returns>Usuario creado.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(UserCreateDto dto)
        {
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Role = dto.Role
            };

            var createdUser = await _userService.CreateUser(user, dto.Password);
            return CreatedAtAction(nameof(GetById),
                new { id = createdUser.Id },
                new UserResponseDto(createdUser));
        }

        /// <summary>
        /// Actualiza los datos de un usuario existente.
        /// </summary>
        /// <param name="id">Identificador del usuario.</param>
        /// <param name="dto">Datos actualizados del usuario.</param>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UserUpdateDto dto)
        {
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Role = dto.Role
            };

            await _userService.UpdateUser(id, user);
            return NoContent();
        }

        /// <summary>
        /// Elimina un usuario por su identificador.
        /// </summary>
        /// <param name="id">Identificador del usuario.</param>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteUser(id);
            return NoContent();
        }
    }
}
