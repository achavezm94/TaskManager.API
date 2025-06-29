using TaskManager.API.Models;
using TaskStatus = TaskManager.API.Models.TaskStatus;

namespace TaskManager.API.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<ProjectTask>> GetTasksByRoleAsync(string role, int? userId = null);
        Task<int> CountTasksByRoleAsync(string role, int? userId);
        Task<ProjectTask> CreateTaskAsync(ProjectTask task);
        Task UpdateTaskAsync(int id, ProjectTask task);
        Task UpdateTaskStatusAsync(int id, TaskStatus status);
        Task UpdateTaskStatusAsync(int id, int assign);
        Task DeleteTaskAsync(int id);
    }
}
