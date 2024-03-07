using System.ComponentModel.DataAnnotations;

namespace planner
{
    public class ProjectStatus
    {
        [Key]
        public int ProjectStatusId { get; set; }

        [Required]
        [StringLength(100)]
        public string? ProjectStatusName { get; set; }

        [Required]
        public bool IsFinalStatus { get; set; }
    }
}
