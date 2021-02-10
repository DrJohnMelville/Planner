using Melville.MVVM.RunShellCommands;
using Planner.Models.Appointments;
using Planner.Models.Time;
using Planner.Wpf.PlannerPages;

namespace Planner.Wpf.Appointments
{
    public class SingleAppointmentViewModel: RichTextCommandTarget
    {
        public Appointment Appointment { get; }

        public SingleAppointmentViewModel(
            IPlannerNavigator navigator, 
            IRunShellCommand commandObject, 
            IUsersClock clock,
            Appointment appointment) : 
            base(navigator, commandObject, 
                clock.InstantToLocalDateTime(appointment.Start).Date)
        {
            Appointment = appointment;
        }
    }
}