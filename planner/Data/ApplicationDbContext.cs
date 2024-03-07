using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using planner.Models;

namespace planner.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : IdentityDbContext<AppUser>(options) 
    {
        private readonly IConfiguration _configuration = configuration;

        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<RequestTeamAccess> RequestTeamAccesses { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ProjectStatus> ProjectStatuses { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(_configuration.GetConnectionString("DbConnection"));
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Workflow>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<Project>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Task>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<Note>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<Team>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<RequestTeamAccess>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<Client>()
                .HasKey(t => t.ClientId);

            modelBuilder.Entity<ProjectStatus>()
                .HasKey(t => t.ProjectStatusId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
