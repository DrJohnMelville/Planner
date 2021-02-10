using System.Collections.Generic;
using NodaTime;
using Planner.Models.Appointments;
using Planner.Models.Repositories;
using Planner.Wpf.PlannerPages;

namespace Planner.Wpf.Appointments
{
    public class DailyAppointmentsViewModel
    {
        private LocalDate date;
        private IPlannerNavigator navigator;
        public IList<Appointment> Appointments { get; }
        

        public DailyAppointmentsViewModel(LocalDate date,
            ILocalRepository<Appointment> appointmentFactory, IPlannerNavigator navigator)
        {
            this.date = date;
            this.navigator = navigator;
            Appointments = appointmentFactory.ItemsForDate(date);
        }

        public void AppointmentLinkClicked(Appointment appointment)
        {
            navigator.ToAppointmentPage(appointment);
        }
    }
}