using System;
using System.Collections;
using System.Collections.Generic;
using NodaTime;

namespace Planner.Models.Appointments.SuncStructure
{
    public class AppointmentSyncInfo
    {
        public IList<string> KeysToDelete { get; set; } = Array.Empty<string>();
        public IList<SyncAppointmentData> Items { get; set; } = Array.Empty<SyncAppointmentData>();
    }

    public class SyncAppointmentData
    {
        public string Title { get; set; } = "";
        public string Location { get; set; } = "";
        public string BodyText { get; set; } = "";
        public string UniqueOutlookId { get; set; } = "";

        public IList<SyncAppointmentTime> Times { get; set; } = Array.Empty<SyncAppointmentTime>();

    }

    public class SyncAppointmentTime
    {
        public Instant StartTime { get; set; }
        public Instant EndTime { get; set; }
    }
}