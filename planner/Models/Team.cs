using System.ComponentModel.DataAnnotations;
namespace planner
{
    public class Team
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;
        public List<string>? EmailRequestor { get; set; }
    }
}
