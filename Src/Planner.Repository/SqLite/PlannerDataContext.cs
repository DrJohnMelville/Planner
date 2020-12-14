using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Planner.Models.Appointments;
using Planner.Models.Appointments.SyncStructure;
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
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<AppointmentDetails> AppointmentDetails => Set<AppointmentDetails>();
        public DbSet<SyncTime> SyncTimes => Set<SyncTime>();

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

            modelBuilder.Entity<AppointmentDetails>().HasKey(i => i.AppointmentDetailsId);
            modelBuilder.Entity<AppointmentDetails>().Property(i => i.AppointmentDetailsId).ValueGeneratedNever();
            modelBuilder.Entity<AppointmentDetails>().HasIndex(i => i.UniqueOutlookId);
            modelBuilder.Entity<Appointment>().HasKey(i => new {i.AppointmentDetailsId, i.Start});
            modelBuilder.Entity<Appointment>().Property(i => i.AppointmentDetailsId).ValueGeneratedNever();
            modelBuilder.Entity<Appointment>().Property(i => i.Start).ValueGeneratedNever();
            modelBuilder.Entity<AppointmentDetails>().HasMany(i => i.Appointments).WithOne(i => i.AppointmentDetails!)
                .IsRequired().OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SyncTime>().HasKey(i => i.SyncTimeId);
            modelBuilder.Entity<SyncTime>().Property(i => i.SyncTimeId).ValueGeneratedNever();

        }

        private static void DeclareTable<T>(EntityTypeBuilder<T> entity) where T:PlannerItemWithDate
        {
            entity.HasKey(i => i.Key);
            entity.HasIndex(i => i.Date);
            entity.Property(i => i.Key).ValueGeneratedNever();
        }
    }
}