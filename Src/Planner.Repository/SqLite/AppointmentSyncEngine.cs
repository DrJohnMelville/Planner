﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Planner.Models.Appointments;
using Planner.Models.Appointments.SuncStructure;

namespace Planner.Repository.SqLite
{
    public class AppointmentSyncEngine
    {
        private readonly Func<PlannerDataContext> contextFactory;

        public AppointmentSyncEngine(Func<PlannerDataContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task Synchronize(AppointmentSyncInfo info)
        {
            await using var context = contextFactory();
            
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
    }
}