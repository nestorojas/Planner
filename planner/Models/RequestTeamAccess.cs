using System.ComponentModel.DataAnnotations;

namespace planner
{
    public class RequestTeamAccess
    {
        [Key]
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string? EmailOwner { get; set; }
        public string? UserRequestorEmail { get; set; }
    }
}
