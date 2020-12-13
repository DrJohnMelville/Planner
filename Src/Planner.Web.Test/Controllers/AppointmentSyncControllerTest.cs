using System.Threading.Tasks;
using Microsoft.Extensions.ObjectPool;
using Moq;
using Planner.Models.Appointments.SyncStructure;
using Planner.Repository.SqLite;
using Planner.Web.Controllers;
using Xunit;

namespace Planner.Web.Test.Controllers
{
    public class AppointmentSyncControllerTest
    {
        private readonly Mock<IAppointmentSyncEngine> engine = new();
        private readonly AppointmentSyncController sut;

        public AppointmentSyncControllerTest()
        {
            sut = new AppointmentSyncController(engine.Object);
        }

        [Fact]
        public async Task PushData()
        {
            var dat = new AppointmentSyncInfo();
            await sut.PushOutlookData(dat);
            engine.Verify(i=>i.Synchronize(dat), Times.Once);
            engine.VerifyNoOtherCalls();
        }

    }
}