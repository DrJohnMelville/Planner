using System;
using System.Collections;
using System.Collections.Generic;
using NodaTime;
using Planner.Models.Appointments;
using Planner.Models.Repositories;

namespace Planner.WpfViewModels.Appointments
{
    public class DailyAppointmentsViewModel
    {
        private LocalDate date;
        public IList<Appointment> Appointments { get; }

        public DailyAppointmentsViewModel(LocalDate date,
            ILocalRepository<Appointment> appointmentFactory)
        {
            this.date = date;
            Appointments = appointmentFactory.ItemsForDate(date);
        }
    }
}