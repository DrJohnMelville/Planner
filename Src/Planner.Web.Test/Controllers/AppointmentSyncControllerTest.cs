using System.Threading.Tasks;
using Moq;
using NodaTime;
using Planner.Models.Appointments.SyncStructure;
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

        [Fact]
        public async Task ClearTest()
        {
            await sut.Clear();
            engine.Verify(i=>i.ClearAppointments());
            engine.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetLastTime()
        {
            engine.Setup(i => i.LastSynchronizationTime()).ReturnsAsync(Instant.FromUnixTimeSeconds(12345));
            Assert.Equal(12345, await sut.GetLastUpdateTime());
            
        }


    }
}