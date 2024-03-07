using System.ComponentModel.DataAnnotations;
namespace planner
{
    public class Task
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ProjectId { get; set; }
        public List<int>? PredecessorId { get; set; }
        public string AssignedUserId { get; set; } = string.Empty;
        public string CreatedByUserId { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; } = new DateTime();
        public DateTime? DueDate { get; set; } = new DateTime?();
        public DateTime? CompletedDate { get; set; } = new DateTime?();
        public bool IsWorkflow { get; set; }
        public int? IdTemplate {  get; set; }
        public int? WorkflowId { get; set; } = null;
    }
}
