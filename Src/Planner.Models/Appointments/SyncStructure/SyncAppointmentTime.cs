using NodaTime;

namespace Planner.Models.Appointments.SyncStructure
{
    public class SyncAppointmentTime
    {
        public Instant StartTime { get; set; }
        public Instant EndTime { get; set; }
        public Appointment ToAppointment() => new() {Start = StartTime, End = EndTime};
    }
}