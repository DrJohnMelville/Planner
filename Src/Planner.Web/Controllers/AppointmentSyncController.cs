
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Planner.Models.Appointments.SyncStructure;
using Planner.Repository.SqLite;

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
    }
}