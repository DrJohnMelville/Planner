using System;
using System.Collections;
using System.Collections.Generic;
using Melville.MVVM.Wpf.DiParameterSources;
using NodaTime;
using Planner.Models.Appointments;
using Planner.Models.Repositories;
using Planner.WpfViewModels.PlannerPages;

namespace Planner.WpfViewModels.Appointments
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