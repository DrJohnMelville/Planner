using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NodaTime;

namespace Planner.Models.Appointments.SuncStructure
{
    public class AppointmentSyncInfo
    {
        public IList<string> KeysToDelete { get; set; } = Array.Empty<string>();
        public IList<SyncAppointmentData> Items { get; set; } = Array.Empty<SyncAppointmentData>();
        
        public List<string> DeletedAndModifiedItemOutlookIds() =>
            KeysToDelete.Concat(OutlookIdsForPotentiallyModifiedItems()).ToList();

        private IEnumerable<string> OutlookIdsForPotentiallyModifiedItems() =>
            Items
                .Select(i=>i.UniqueOutlookId)
                .Where(i=>!string.IsNullOrWhiteSpace(i));
    }

    public class SyncAppointmentData
    {
        public string Title { get; set; } = "";
        public string Location { get; set; } = "";
        public string BodyText { get; set; } = "";
        public string UniqueOutlookId { get; set; } = "";

        public IList<SyncAppointmentTime> Times { get; set; } = Array.Empty<SyncAppointmentTime>();

        public AppointmentDetails ToAppointmentDetails() =>
            new()
            {
                AppointmentDetailsId = Guid.NewGuid(),
                Title = Title,
                Location = Location,
                BodyText = BodyText,
                UniqueOutlookId = UniqueOutlookId,
                Appointments = Times
                    .Select(i=>i.ToAppointment()).ToList()
            };
    }

    public class SyncAppointmentTime
    {
        public Instant StartTime { get; set; }
        public Instant EndTime { get; set; }
        public Appointment ToAppointment() => new() {Start = StartTime, End = EndTime};
    }
}