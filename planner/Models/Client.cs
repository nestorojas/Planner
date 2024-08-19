using System.ComponentModel.DataAnnotations;
namespace planner
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }

        [StringLength(100)]
        public string? ClientName { get; set; }

        [StringLength(100)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(10)]
        public string? PostalCode { get; set; }

        [StringLength(20)]
        public string? Phone1 { get; set; }

        [StringLength(20)]
        public string? Phone2 { get; set; }

        [StringLength(100)]
        public string? Email1 { get; set; }

        [StringLength(100)]
        public string? Email2 { get; set; }
        public bool IsActive { get; set; }
        public int? TeamId { get; set; }
    }

}
