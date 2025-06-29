using TaskStatus = TaskManager.API.Models.TaskStatus;
namespace TaskManager.API.DTOs.Task
{
    public class TaskUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.Pending;
        public int AssignedUserId { get; set; }
    }
}
