using TaskStatus = TaskManager.API.Models.TaskStatus;
namespace TaskManager.API.DTOs.Task
{
    public class TaskStatusUpdateDto
    {
        public TaskStatus Status { get; set; }
    }
}
