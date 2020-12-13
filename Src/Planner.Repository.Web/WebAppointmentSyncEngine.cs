using System.Threading.Tasks;
using Planner.Models.Appointments.SyncStructure;

namespace Planner.Repository.Web
{
    public class WebAppointmentSyncEngine: IAppointmentSyncEngine
    {
        private readonly IJsonWebService service;

        public WebAppointmentSyncEngine(IJsonWebService service)
        {
            this.service = service;
        }

        public Task Synchronize(AppointmentSyncInfo info)
        {
            return service.Put("/AppointmentSync/Outlook", info);
        }
    }
}