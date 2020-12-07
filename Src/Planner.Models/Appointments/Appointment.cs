using System;
using Melville.INPC;
using NodaTime;

namespace Planner.Models.Appointments
{
    public partial class Appointment
    {
        [AutoNotify] private Instant start;
        [AutoNotify] private Instant end;
        [AutoNotify] private Guid details;
    }

    public partial class AppointmentDetails
    {
        [AutoNotify] private string title = "";
        [AutoNotify] private string location = "";
        [AutoNotify] private string bodyText = "";
        [AutoNotify] private string uniqueOutlookId = "";
        
    }
}