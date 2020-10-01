using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NodaTime;
using Planner.Models.Repositories;
using Planner.Web.Controllers;
using Xunit;

namespace Planner.Web.Test.Controllers
{
    public class PlannerTaskControllerTest
    {
        private readonly Mock<IRemotePlannerTaskRepository> repo = new Mock<IRemotePlannerTaskRepository>();
        private readonly PlannerTaskController sut;
        private readonly LocalDate date = new LocalDate(1975, 7, 28);

        public PlannerTaskControllerTest()
        {
            sut = new PlannerTaskController(repo.Object);
        }

        [Fact]
        public async Task GetItems()
        {
            repo.Setup(i => i.TasksForDate(date)).Returns(new[]
            {
                new RemotePlannerTask(Guid.Empty) {Name = "Foo"},
                new RemotePlannerTask(Guid.Empty) {Name = "Bar"},
            }.ToAsyncEnumerable());

            var ret = await sut.TasksForDate(date).ToListAsync();
        }
    }
}