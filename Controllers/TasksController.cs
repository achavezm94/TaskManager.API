using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.API.Data;
using TaskManager.API.DTOs.Task;
using TaskManager.API.Models;
using TaskManager.API.Services;

namespace TaskManager.API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de tareas.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController(ITaskService taskService, AppDbContext context) : ControllerBase
    {
        private readonly ITaskService _taskService = taskService;
        private readonly AppDbContext _context = context;

        /// <summary>
        /// Obtiene todas las tareas según el rol del usuario autenticado.
        /// </summary>
        /// <returns>Lista de tareas accesibles para el usuario.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "Employee";
            int? userId = null;

            if (role == "Employee")
            {
                userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }

            var tasks = await _taskService.GetTasksByRoleAsync(role, userId);
            return Ok(tasks.Select(t => new TaskResponseDto(t)));
        }

        /// <summary>
        /// Obtiene todas las tareas según el rol del usuario autenticado.
        /// </summary>
        /// <returns>Lista de tareas accesibles para el usuario.</returns>
        [HttpGet("count")]
        public async Task<IActionResult> GetAllCount()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "Employee";
            int? userId = null;

            if (role == "Employee")
            {
                userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }

            var count = await _taskService.CountTasksByRoleAsync(role, userId);
            return Ok(count);
        }

        /// <summary>
        /// Crea una nueva tarea.
        /// Solo accesible para administradores y supervisores.
        /// </summary>
        /// <param name="dto">Datos de la tarea a crear.</param>
        /// <returns>La tarea creada.</returns>
        /// <response code="201">Tarea creada exitosamente.</response>
        [HttpPost]
        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<IActionResult> Create(TaskCreateDto dto)
        {
            var task = new ProjectTask
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                Status = dto.Status,
                AssignedUserId = dto.AssignedUserId
            };

            var createdTask = await _taskService.CreateTaskAsync(task);
            return CreatedAtAction(nameof(GetById),
                new { id = createdTask.Id },
                new TaskResponseDto(createdTask));
        }

        /// <summary>
        /// Obtiene una tarea por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la tarea.</param>
        /// <returns>La tarea solicitada.</returns>
        /// <response code="404">No se encontró la tarea.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var task = await _context.ProjectTasks
                .Include(t => t.AssignedUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null) return NotFound();

            return Ok(new TaskResponseDto(task));
        }

        /// <summary>
        /// Actualiza una tarea existente.
        /// Solo accesible para administradores y supervisores.
        /// </summary>
        /// <param name="id">Identificador de la tarea.</param>
        /// <param name="dto">Datos actualizados de la tarea.</param>
        /// <response code="204">Tarea actualizada exitosamente.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<IActionResult> Update(int id, TaskUpdateDto dto)
        {
            var task = new ProjectTask
            {
                Id = id,
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                Status = dto.Status,
                AssignedUserId = dto.AssignedUserId
            };

            await _taskService.UpdateTaskAsync(id, task);
            return NoContent();
        }

        /// <summary>
        /// Actualiza el estado de una tarea.
        /// Accesible para administradores, supervisores y empleados.
        /// </summary>
        /// <param name="id">Identificador de la tarea.</param>
        /// <param name="dto">Nuevo estado de la tarea.</param>
        /// <response code="204">Estado actualizado exitosamente.</response>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Supervisor,Employee")]
        public async Task<IActionResult> UpdateStatus(int id, TaskStatusUpdateDto dto)
        {
            await _taskService.UpdateTaskStatusAsync(id, dto.Status);
            return NoContent();
        }

        /// <summary>
        /// Actualiza la asignacion de una tarea.
        /// Accesible para administradores, supervisores.
        /// </summary>
        /// <param name="id">Identificador de la tarea.</param>
        /// <param name="dto">Nuevo asignacion de la tarea.</param>
        /// <response code="204">Estado actualizado exitosamente.</response>
        [HttpPatch("{id}/{assign}")]
        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<IActionResult> UpdateAssingTask(int id, int assign)
        {
            await _taskService.UpdateTaskStatusAsync(id, assign);
            return NoContent();
        }

        /// <summary>
        /// Elimina una tarea por su identificador.
        /// Solo accesible para administradores.
        /// </summary>
        /// <param name="id">Identificador de la tarea.</param>
        /// <response code="204">Tarea eliminada exitosamente.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _taskService.DeleteTaskAsync(id);
            return NoContent();
        }
    }
}
