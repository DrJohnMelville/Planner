using System.Collections.Generic;
using System.Threading.Tasks;
using Melville.MVVM.Wpf.DiParameterSources;
using NodaTime;
using Planner.Models.Appointments;
using Planner.Models.Appointments.SyncStructure;
using Planner.Models.HtmlGeneration;
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

        public async Task ClearAllAppointments(
            [FromServices]IAppointmentSyncEngine engine,
            [FromServices]IEventBroadcast<ClearCachesEventArgs> signalObject)
        {
            await engine.ClearAppointments();
            signalObject.Fire(this, new ClearCachesEventArgs());
        }
    }
}