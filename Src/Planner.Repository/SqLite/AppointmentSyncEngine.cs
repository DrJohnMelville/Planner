using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.Appointments;
using Planner.Models.Appointments.SyncStructure;

namespace Planner.Repository.SqLite
{
    public class AppointmentSyncEngine : IAppointmentSyncEngine
    {
        private readonly Func<PlannerDataContext> contextFactory;
        private readonly IClock clock;

        public AppointmentSyncEngine(Func<PlannerDataContext> contextFactory, IClock clock)
        {
            this.contextFactory = contextFactory;
            this.clock = clock;
        }

        public async Task Synchronize(AppointmentSyncInfo info)
        {
            await using var context = contextFactory();
            
            WriteLastSynchronizationTime(clock.GetCurrentInstant());
            await DeleteAppointments(context, info.DeletedAndModifiedItemOutlookIds());
            CopyAppointments(context, info.Items);

            await context.SaveChangesAsync();
        }
        
        private Task<int> DeleteAppointments(PlannerDataContext context, IList<string> infoKeysToDelete) =>
            ((IQueryable<AppointmentDetails>)context.AppointmentDetails)
            .Where(i => infoKeysToDelete.Contains(i.UniqueOutlookId))
            .DeleteFromQueryAsync();

        private void CopyAppointments(
            PlannerDataContext context, IEnumerable<SyncAppointmentData> newAppointments)
        {
            foreach (var appointment in newAppointments)
            {
                context.AppointmentDetails.Add(appointment.ToAppointmentDetails());
            }
        }

        private Instant lastUpdate = 
            Instant.FromDateTimeUtc(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        public Task<Instant> LastSynchronizationTime()
        {
            return Task.FromResult(lastUpdate);
        }
        private void WriteLastSynchronizationTime(Instant time)
        {
            lastUpdate = time;
        }
    }
}