using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Planner.Models.Blobs;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Planner.Models.Tasks;

namespace Planner.Repository.SqLite
{
    public class PlannerDataContext:DbContext
    {
        public DbSet<PlannerTask> PlannerTasks => Set<PlannerTask>();
        public DbSet<Note> Notes => Set<Note>();
        public DbSet<Blob> Blobs => Set<Blob>();

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
            DeclareTable(modelBuilder.Entity<PlannerTask>());
            DeclareTable(modelBuilder.Entity<Note>());
            DeclareTable(modelBuilder.Entity<Blob>());
            
        }

        private static void DeclareTable<T>(EntityTypeBuilder<T> entity) where T:PlannerItemWithDate
        {
            entity.HasKey(i => i.Key);
            entity.HasIndex(i => i.Date);
            entity.Property(i => i.Key).ValueGeneratedNever();
        }
    }
}