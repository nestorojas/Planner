using System.ComponentModel.DataAnnotations;

namespace planner.ViewModel
{
    public class TaskVM
    {
        public int TaskId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public List<int>? PredecessorId { get; set; }
        public string AssignedUserId { get; set; } = string.Empty;
        public string AssignedUserName { get; set; } = string.Empty;
        public string CreatedByUserId { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; } = new DateTime();
        public DateTime? DueDate { get; set; } = new DateTime?();
        public DateTime? CompletedDate { get; set; } = new DateTime?();
        public bool IsWorkflow { get; set; }
        public int? IdTemplate { get; set; }
        public int? WorkflowId { get; set; } = null;
        public int? Duration { get; set; } = null;
        [Required]
        public string? Note { get; set; }
        public string? Mode { get; set; }
    }
}
