using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TaskManager.API.Models
{
    public enum Role
    {
        Admin,
        Supervisor,
        Employee
    }

    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public Role Role { get; set; } = Role.Employee;

        [JsonIgnore]
        public List<ProjectTask> Tasks { get; set; } = [];
    }
}