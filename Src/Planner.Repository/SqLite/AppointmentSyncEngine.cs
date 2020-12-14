using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Planner.Models.Appointments;
using Planner.Models.Appointments.SyncStructure;

namespace Planner.Repository.SqLite
{
    public class AppointmentSyncEngine : IAppointmentSyncEngine
    {
        private readonly Func<PlannerDataContext> contextFactory;

        public AppointmentSyncEngine(Func<PlannerDataContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task Synchronize(AppointmentSyncInfo info)
        {
            await using var context = contextFactory();
            
            await WriteLastSynchronizationTime(context, info.QueryTime);
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

        private static readonly Guid lastUpdateTimeKey = new Guid("015A2146-2949-40F6-8117-B8BCBAC9C21D");
        public async Task<Instant> LastSynchronizationTime()
        {
            await using var context = contextFactory();
            var lastSyncRecord = await context.SyncTimes.AsNoTracking()
                .FirstOrDefaultAsync(i => i.SyncTimeId == lastUpdateTimeKey);
            return lastSyncRecord?.Time ?? 
                   Instant.FromDateTimeUtc(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }
        private Task WriteLastSynchronizationTime(PlannerDataContext context, Instant time) => 
            context.Upsert(CreateSyncRecord(time)).RunAsync();

        private static SyncTime CreateSyncRecord(Instant time) => 
            new() {SyncTimeId = lastUpdateTimeKey, Time = time};
    }
}