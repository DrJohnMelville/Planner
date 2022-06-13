using System;
using System.Collections.Generic;
using Melville.INPC;
using NodaTime;

namespace Planner.Models.Appointments
{
    [Obsolete]
    public partial class Appointment
    {
        [AutoNotify] private Instant start;
        [AutoNotify] private Instant end;
        [AutoNotify] private Guid appointmentDetailsId;

        [AutoNotify] private AppointmentDetails? appointmentDetails;
    }

    [Obsolete]
    public partial class AppointmentDetails
    {
        [AutoNotify] private Guid appointmentDetailsId;
        [AutoNotify] private string title = "";
        [AutoNotify] private string location = "";
        [AutoNotify] private string bodyText = "";
        [AutoNotify] private string uniqueOutlookId = "";

        [AutoNotify] private IList<Appointment>? appointments;
    }
}