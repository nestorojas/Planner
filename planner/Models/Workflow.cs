using System.ComponentModel.DataAnnotations;

namespace planner
{
    public class Workflow
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string CreatedByUserId { get; set; } = string.Empty;
        public string? OwnerId { get; set; }
    }
}
