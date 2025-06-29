using TaskManager.API.Models;

namespace TaskManager.API.DTOs.Task
{
    public class TaskResponseDto(ProjectTask task)
    {
        public int Id { get; set; } = task.Id;
        public string Title { get; set; } = task.Title;
        public string? Description { get; set; } = task.Description;
        public DateTime CreatedAt { get; set; } = task.CreatedAt;
        public DateTime? DueDate { get; set; } = task.DueDate;
        public string Status { get; set; } = task.Status.ToString();
        public int AssignedUserId { get; set; } = task.AssignedUserId;
        public string? AssignedUserName { get; set; } = task.AssignedUser?.Name;
    }
}
