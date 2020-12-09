using NodaTime;

namespace Planner.WpfViewModels.Appointments
{
    public class DailyAppointmentsViewModel
    {
        private LocalDate date;

        public DailyAppointmentsViewModel(LocalDate date)
        {
            this.date = date;
        }
    }
}