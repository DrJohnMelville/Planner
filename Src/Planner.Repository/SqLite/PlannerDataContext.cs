using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Planner.Models.Notes;
using Planner.Models.Tasks;

namespace Planner.Repository.SqLite
{
    public class PlannerDataContext:DbContext
    {
        public DbSet<PlannerTask> PlannerTasks { get; set; } = null!;
        public DbSet<Note> Notes { get; set; } = null!;

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
            modelBuilder.Entity<PlannerTask>().HasKey(i => i.Key);
            modelBuilder.Entity<PlannerTask>().HasIndex(i => i.Date);
            modelBuilder.Entity<PlannerTask>().Property(i => i.Key).ValueGeneratedNever();

            modelBuilder.Entity<Note>().HasKey(i => i.Key);
            modelBuilder.Entity<Note>().HasIndex(i => i.DisplaysOnDate);
            modelBuilder.Entity<Note>().Property(i=>i.Key).ValueGeneratedNever();
        }
    }
}