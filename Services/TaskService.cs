using Microsoft.EntityFrameworkCore;
using TaskManager.API.Data;
using TaskManager.API.Models;
using TaskStatus = TaskManager.API.Models.TaskStatus;

namespace TaskManager.API.Services
{
    public class TaskService(AppDbContext context) : ITaskService
    {
        private readonly AppDbContext _context = context;

        public async Task<ProjectTask> CreateTaskAsync(ProjectTask task)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == task.AssignedUserId);
            if (!userExists) throw new Exception("Usuario asignado no existe");

            _context.ProjectTasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<int> CountTasksByRoleAsync(string role, int? userId)
        {
            var query = _context.ProjectTasks.AsQueryable();

            if (role == "Employee" && userId.HasValue)
            {
                query = query.Where(t => t.AssignedUserId == userId.Value);
            }
            return await query.CountAsync();
        }


        public async Task DeleteTaskAsync(int id)
        {
            var task = await _context.ProjectTasks.FindAsync(id);
            if (task == null) return;

            _context.ProjectTasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProjectTask>> GetTasksByRoleAsync(string role, int? userId = null)
        {
            IQueryable<ProjectTask> query = _context.ProjectTasks.Include(t => t.AssignedUser);

            if (role == "Employee" && userId.HasValue)
            {
                query = query.Where(t => t.AssignedUserId == userId.Value);
            }

            return await query.ToListAsync();
        }

        public async Task UpdateTaskAsync(int id, ProjectTask updatedTask)
        {
            var task = await _context.ProjectTasks.FindAsync(id) ?? throw new Exception("Tarea no encontrada");
            if (await _context.Users.AllAsync(u => u.Id != updatedTask.AssignedUserId))
                throw new Exception("Usuario asignado no válido");

            task.Title = updatedTask.Title;
            task.Description = updatedTask.Description;
            task.DueDate = updatedTask.DueDate;
            task.Status = updatedTask.Status;
            task.AssignedUserId = updatedTask.AssignedUserId;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaskStatusAsync(int id, TaskStatus status)
        {
            var task = await _context.ProjectTasks.FindAsync(id) ?? throw new Exception("Tarea no encontrada");
            if (!Enum.IsDefined(typeof(TaskStatus), status))
                throw new ArgumentException("Estado no válido");

            task.Status = status;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaskStatusAsync(int id, int assign)
        {
            var task = await _context.ProjectTasks.FindAsync(id) ?? throw new Exception("Tarea no encontrada");            
            task.AssignedUserId = assign;
            await _context.SaveChangesAsync();
        }
    }
}