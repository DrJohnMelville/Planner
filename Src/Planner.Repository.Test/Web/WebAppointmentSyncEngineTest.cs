using System;
using System.Net.Http;
using System.Threading.Tasks;
using Melville.TestHelpers.Http;
using Moq;
using Planner.Models.Appointments.SyncStructure;
using Planner.Repository.Web;
using Xunit;

namespace Planner.Repository.Test.Web
{
    public class WebAppointmentSyncEngineTest: TestWithJsonWebService
    {
        private readonly WebAppointmentSyncEngine sut;

        public WebAppointmentSyncEngineTest()
        {
            sut = new WebAppointmentSyncEngine(service);
        }

        [Fact]
        public async Task CallWebSynchronize()
        {
            var data = new AppointmentSyncInfo();
            httpSource.Setup(i => true, HttpMethod.Put).ReturnsJson("{}");
            await sut.Synchronize(data);
            httpSource.Verify((Func<string,bool>)(i=>i.EndsWith("/AppointmentSync/Outlook")), 
                HttpMethod.Put, Times.Once);
        }

    }
}