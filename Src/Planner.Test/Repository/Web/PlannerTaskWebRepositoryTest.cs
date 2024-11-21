using Melville.TestHelpers.Http;
using NodaTime;
using Planner.Models.Tasks;
using Planner.Repository.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TUnit.Repository.Web;

    public class PlannerTaskWebRepositoryTest: TestWithJsonWebService
    {
        private readonly WebRepository<PlannerTask> sut;

        public PlannerTaskWebRepositoryTest(): base()
        {
            sut = new WebRepository<PlannerTask>(service, "Task");
        }

        [Test]
        public async Task GetPlannerTasks()
        {
            httpSource.Setup(i=>i.EndsWith($"Task/1975-07-28/{DateTimeZone.Utc.Id}"), HttpMethod.Get).
                ReturnsJson("[{\"Date\":\"1975-07-28\",\"Name\":\"My Birthday\",\"Priority\":\" \",\"Order\":0,\"Status\":0,\"StatusDetail\":\"\"}," +
                            "{\"Date\":\"1975-07-28\",\"Name\":\"Second Task\",\"Priority\":\" \",\"Order\":0,\"Status\":0,\"StatusDetail\":\"\"}]");
            var list = await sut.TasksForDate(date, DateTimeZone.Utc).ToListAsync();
            Assert.Equal(2, list.Count);
            Assert.Equal("My Birthday", list[0].Name);
            Assert.Equal("Second Task", list[1].Name);
        }
        [Test]
        public async Task QueryByKeys()
        {
            httpSource.Setup(i=>i.EndsWith("Task/Query"), HttpMethod.Get).
                ReturnsJson("[{\"Date\":\"1975-07-28\",\"Name\":\"My Birthday\",\"Priority\":\" \",\"Order\":0,\"Status\":0,\"StatusDetail\":\"\"}," +
                            "{\"Date\":\"1975-07-28\",\"Name\":\"Second Task\",\"Priority\":\" \",\"Order\":0,\"Status\":0,\"StatusDetail\":\"\"}]");
            var list = await sut.ItemsFromKeys(new []
            {
                Guid.Empty, Guid.Empty
            }).ToListAsync();
            Assert.Equal(2, list.Count);
            Assert.Equal("My Birthday", list[0].Name);
            Assert.Equal("Second Task", list[1].Name);
        }

        [Test]
        public async Task DeleteTask()
        {
            httpSource.Setup(i=>true, HttpMethod.Delete).ReturnsJson("");
            var guid = Guid.NewGuid();
            await sut.Delete(new PlannerTask(guid));
            httpSource.Verify((Func<string,bool>)(i=>i.EndsWith("/Task/"+guid)), 
                HttpMethod.Delete, Times.Once);
        }

        [Test]
        public async Task PostAddMethod()
        {
            httpSource.Setup(i=>true, HttpMethod.Post).ReturnsJson("");
            await sut.Add(new PlannerTask(Guid.NewGuid()));
            httpSource.Verify((Func<string,bool>)(i=>i.EndsWith("/Task")), 
                HttpMethod.Post, Times.Once);
        }
        [Test]
        public async Task PutChangeMethod()
        {
            httpSource.Setup(i=>true, HttpMethod.Put).ReturnsJson("");
            await sut.Update(new PlannerTask(Guid.NewGuid()));
            httpSource.Verify((Func<string,bool>)(i=>i.EndsWith("/Task")), 
                HttpMethod.Put, Times.Once);
        }
    }