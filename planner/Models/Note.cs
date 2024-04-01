using System.ComponentModel.DataAnnotations;
namespace planner
{
    public class Note
    {
        [Key]
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int ProjectId { get; set; }
        public Task Task { get; set; } = new Task();
        public string Comment { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
