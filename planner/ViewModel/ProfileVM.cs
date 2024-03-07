using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace planner.ViewModel
{
    public class ProfileVM
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int? TeamId { get; set; }
        [DisplayName("Team")]
        public string TeamName { get; set; } = string.Empty;
        public string TeamOwnerEmail { get; set; } = string.Empty;
        public bool IsTeamOwner { get; set; } = false;
        [DisplayName("Awaiting for Response from Team:")]
        public string? TeamRequestAccess { get; set; }
        [Display(Name = "Create Team")]
        public bool CreateTeam { get; set; }
        public List<RequestTeamAccess> TeamAccess { get; set; } = [];
        public List<TeamMembers> TeamMembers { get; set; } = [];
    }
    public class TeamMembers
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
    }
}
