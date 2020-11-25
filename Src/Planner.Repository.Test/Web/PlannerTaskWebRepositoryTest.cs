using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Melville.TestHelpers.Http;
using Moq;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Planner.Models.Notes;
using Planner.Models.Tasks;
using Planner.Repository.Test.SqLite;
using Planner.Repository.Web;
using Xunit;

namespace Planner.Repository.Test.Web
{
    public class TestWithJsonWebService
    {
        protected readonly Mock<IHttpClientMock> httpSource = new Mock<IHttpClientMock>();
        protected readonly IJsonWebService service;
        protected readonly LocalDate date = new LocalDate(1975, 07, 28);

        public TestWithJsonWebService()
        {
            var seropt = new JsonSerializerOptions();
            seropt.IgnoreReadOnlyProperties = true;
            seropt.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            var httpClient = httpSource.ToHttpClient();
            httpClient.BaseAddress = new Uri("https://Planner.DRJohnMelville.com");
            service = new JsonWebService(httpClient, seropt);
        }
    }

    public class WebNoteSearcherTest : TestWithJsonWebService
    {
        private readonly WebNoteSearcher sut;

        public WebNoteSearcherTest(): base()
        {
            sut = new WebNoteSearcher(service);
        }

        [Fact]
        public async Task GetSearch()
        {
            httpSource.Setup(i=>i.EndsWith("SearchNotes/Foo/1975-07-28/1975-07-29"), HttpMethod.Get)
                .ReturnsJson("[{\"Title\":\"Title1\"}]");

            var items = await sut.SearchFor("Foo", date, date.PlusDays(1)).ToListAsync();
            Assert.Single(items);
            Assert.Equal("Title1", items[0].Title);
            
        }

    }
    public class PlannerTaskWebRepositoryTest: TestWithJsonWebService
    {
        private readonly WebRepository<PlannerTask> sut;

        public PlannerTaskWebRepositoryTest(): base()
        {
            sut = new WebRepository<PlannerTask>(service, "Task");
        }

        [Fact]
        public async Task GetPlannerTasks()
        {
            httpSource.Setup(i=>i.EndsWith("Task/1975-07-28"), HttpMethod.Get).
                ReturnsJson("[{\"Date\":\"1975-07-28\",\"Name\":\"My Birthday\",\"Priority\":\" \",\"Order\":0,\"Status\":0,\"StatusDetail\":\"\"}," +
                            "{\"Date\":\"1975-07-28\",\"Name\":\"Second Task\",\"Priority\":\" \",\"Order\":0,\"Status\":0,\"StatusDetail\":\"\"}]");
            var list = await sut.TasksForDate(date).ToListAsync();
            Assert.Equal(2, list.Count);
            Assert.Equal("My Birthday", list[0].Name);
            Assert.Equal("Second Task", list[1].Name);
        }

        [Fact]
        public async Task DeleteTask()
        {
            httpSource.Setup(i=>true, HttpMethod.Delete).ReturnsJson("");
            var guid = Guid.NewGuid();
            await sut.Delete(new PlannerTask(guid));
            httpSource.Verify((Func<string,bool>)(i=>i.EndsWith("/Task/"+guid)), 
                HttpMethod.Delete, Times.Once);
        }

        [Fact]
        public async Task PostAddMethod()
        {
            httpSource.Setup(i=>true, HttpMethod.Post).ReturnsJson("");
            await sut.Add(new PlannerTask(Guid.NewGuid()));
            httpSource.Verify((Func<string,bool>)(i=>i.EndsWith("/Task")), 
                HttpMethod.Post, Times.Once);
        }
        [Fact]
        public async Task PutChangeMethod()
        {
            httpSource.Setup(i=>true, HttpMethod.Put).ReturnsJson("");
            await sut.Update(new PlannerTask(Guid.NewGuid()));
            httpSource.Verify((Func<string,bool>)(i=>i.EndsWith("/Task")), 
                HttpMethod.Put, Times.Once);
        }
    }
}