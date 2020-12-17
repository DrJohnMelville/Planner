using System.Drawing.Printing;
using Melville.MVVM.RunShellCommands;
using Microsoft.VisualBasic.ApplicationServices;
using NodaTime;
using Planner.Models.Appointments;
using Planner.Models.Time;
using Planner.WpfViewModels.PlannerPages;

namespace Planner.WpfViewModels.Appointments
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