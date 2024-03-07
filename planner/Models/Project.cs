using System.ComponentModel.DataAnnotations;
namespace planner
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        [StringLength(100)]
        public string Initiator { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string OwnerId { get; set; } = string.Empty;

        [Required]
        public int ClientId { get; set; }  // Assuming ClientId is an integer referencing the Client table

        [Required]
        [StringLength(100)]
        public string ClientName { get; set; } = string.Empty;

        public int WorkflowId { get; set; }
        [Required]
        public int ProjectStatusId { get; set; }
        public int TeamId { get; set; } = 0;
    }
}
