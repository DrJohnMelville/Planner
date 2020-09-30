using Microsoft.EntityFrameworkCore;
using Planner.Models.Repositories;
using Planner.Models.Tasks;

namespace Planner.Repository.SqLite
{
    public class PlannerDataContext:DbContext
    {
        public DbSet<RemotePlannerTask> PlannerTasks { get; set; } = null!;

        public PlannerDataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<RemotePlannerTask>()
                .HasKey(i => i.Key);
            modelBuilder.Entity<RemotePlannerTask>().HasIndex(i => i.Date);
        }
    }
}