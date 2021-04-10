using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Planner.Models.Appointments.SyncStructure;

namespace Planner.Web.Controllers
{
    [Route("AppointmentSync")]
    public class AppointmentSyncController
    {
        private readonly IAppointmentSyncEngine engine;

        public AppointmentSyncController(IAppointmentSyncEngine engine)
        {
            this.engine = engine;
        }

        [HttpPut]
        [Route("Outlook")]
        public Task PushOutlookData([FromBody] AppointmentSyncInfo syncInfo) => 
            engine.Synchronize(syncInfo);

        [HttpGet]
        [Route("Last")]
        public async Task<long> GetLastUpdateTime() => (await engine.LastSynchronizationTime()).ToUnixTimeSeconds();

        [HttpPut]
        [Route("Clear")]
        public Task Clear() => engine.ClearAppointments();
    }
}