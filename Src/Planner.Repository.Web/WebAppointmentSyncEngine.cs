using System.Threading.Tasks;
using NodaTime;
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

        public Task Synchronize(AppointmentSyncInfo info) => 
            service.Put("/AppointmentSync/Outlook", info);

        public async Task<Instant> LastSynchronizationTime() =>
            Instant.FromUnixTimeSeconds(await service.Get<long>("/AppointmentSync/Last"));

        public Task ClearAppointments() => 
            service.Put("/AppointmentSync/Clear");
    }
}