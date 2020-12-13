using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Planner.Models.Appointments;
using Planner.Models.Appointments.SuncStructure;

namespace Planner.Repository.SqLite
{
    public class AppointmentSyncEngine
    {
        private readonly PlannerDataContext context;
        public AppointmentSyncEngine(Func<PlannerDataContext> contextFactory)
        {
            context = contextFactory();
        }

        public async Task Synchronize(AppointmentSyncInfo info)
        {
            await DeleteAppointments(
                info.KeysToDelete.Concat(
                    info.Items
                        .Select(i=>i.UniqueOutlookId)
                        .Where(i=>!string.IsNullOrWhiteSpace(i))).ToList());
            CopyAppointments(info);

            await context.SaveChangesAsync();
        }

        private Task<int> DeleteAppointments(IList<string> infoKeysToDelete)
        {
            return ((IQueryable<AppointmentDetails>)context.AppointmentDetails)
                .Where(i => infoKeysToDelete.Contains(i.UniqueOutlookId))
                .DeleteFromQueryAsync();
        }

        private void CopyAppointments(AppointmentSyncInfo info)
        {
            foreach (var appointment in info.Items)
            {
                var apptInfo = CreateSingleAppointment(appointment);
                context.AppointmentDetails.Add(apptInfo);
            }
        }

        private static AppointmentDetails CreateSingleAppointment(SyncAppointmentData appointment)
        {
            var key = Guid.NewGuid();
            var apptInfo = new AppointmentDetails
            {
                AppointmentDetailsId = key,
                Title = appointment.Title,
                Location = appointment.Location,
                BodyText = appointment.BodyText,
                UniqueOutlookId = appointment.UniqueOutlookId,
                Appointments = appointment.Times
                    .Select(CreateAppointmentTime).ToList()
            };
            return apptInfo;
        }

        private static Appointment CreateAppointmentTime(SyncAppointmentTime i) => 
            new() {Start = i.StartTime, End = i.EndTime};
    }
}