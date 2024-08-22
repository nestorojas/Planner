using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace planner
{
    public class AppUser : IdentityUser
    {
        [StringLength(20)]
        [Required]
        public string? Name { get; set; }
        public string? Address { get; set; }
        public int? TeamId { get; set; }
        public bool IsTeamManager { get; set; }
    }
}
