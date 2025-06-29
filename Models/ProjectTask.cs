using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.API.Models
{
    public class ProjectTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DueDate { get; set; }

        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        [ForeignKey("AssignedUserId")]
        public int AssignedUserId { get; set; }
        public User? AssignedUser { get; set; }
    }
}