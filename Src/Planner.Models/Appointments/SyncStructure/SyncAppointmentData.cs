using System;
using System.Collections.Generic;
using System.Linq;

namespace Planner.Models.Appointments.SyncStructure
{
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
}