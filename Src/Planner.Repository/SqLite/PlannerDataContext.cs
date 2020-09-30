using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Planner.Models.Repositories;

namespace Planner.Repository.SqLite
{
    public class PlannerDataContext:DbContext
    {
        public DbSet<RemotePlannerTask> PlannerTasks { get; set; } = null!;

        public PlannerDataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ReplaceService<IValueConverterSelector, LocalDateValueConverterSelector>();
            base.OnConfiguring(optionsBuilder);
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