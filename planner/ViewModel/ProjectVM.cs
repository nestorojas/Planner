using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace planner.ViewModel
{
    public class ProjectVM
    {
        [DisplayName("Project Id")]
        public int ProjectId { get; set; }
        [DisplayName("Project Name")]
        [Required(ErrorMessage = "Project Name is required.")]
        public string? ProjectName { get; set; }
        [DisplayName("Project Description")]
        [Required(ErrorMessage = "Project Description is required.")]
        public string? ProjectDescription { get; set;}
        public int ClientId { get; set; }
        [DisplayName("Client Name")]
        [Required(ErrorMessage = "Client is required.")]
        public string? ClientName { get; set; }
        public int ProjectStatusId { get; set; }
        [DisplayName("Status")]
        public string? ProjectStatus {  get; set; }
        [Required(ErrorMessage = "Type of project is required.")]
        public int WorkflowId { get; set; }
        [DisplayName("Project Type")]
        public string? WorkflowName { get; set; }
        public List<Workflow> Workflows { get; set; } = [];
        public int? TeamId { get; set; }
        public bool IsStartProject { get; set; } = false;
    }
}
