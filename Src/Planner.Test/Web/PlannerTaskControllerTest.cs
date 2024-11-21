using NodaTime;
using Planner.Models.Repositories;
using Planner.Models.Tasks;
using Planner.Web.Controllers;

namespace TUnit.Web;

public class PlannerTaskControllerTest
{
    private readonly Mock<IDatedRemoteRepository<PlannerTask>> repo = new Mock<IDatedRemoteRepository<PlannerTask>>();
    private readonly PlannerTaskController sut;
    private readonly LocalDate date = new LocalDate(1975, 7, 28);

    public PlannerTaskControllerTest()
    {
        sut = new PlannerTaskController(repo.Object);
    }

    [Test]
    public async Task GetItems()
    {
        repo.Setup(i => i.TasksForDate(date, DateTimeZone.Utc)).Returns(new[]
        {
            new PlannerTask(Guid.Empty) {Name = "Foo"},
            new PlannerTask(Guid.Empty) {Name = "Bar"},
        }.ToAsyncEnumerable());

        var ret = await sut.TasksForDate(date, DateTimeZone.Utc.Id).ToListAsync();
        Assert.Equal(2, ret.Count);
        Assert.Equal("Foo", ret[0].Name);
        Assert.Equal("Bar", ret[1].Name);
            
            
    }

    [Test]
    public async Task PutTask()
    {
        var rpt = new PlannerTask(Guid.NewGuid());
        await sut.Update(rpt);
        repo.Verify(i=>i.Update(rpt), Times.Once);
        repo.VerifyNoOtherCalls();
    }
    [Test]
    public async Task PostTask()
    {
        var rpt = new PlannerTask(Guid.NewGuid());
        await sut.Add(rpt);
        repo.Verify(i=>i.Add(rpt), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TestMethod()
    {
        var guid = Guid.NewGuid();
        await sut.DeleteTask(guid);
        repo.Verify(i => i.Delete(It.Is<PlannerTask>(
            rpt=>rpt.Key == guid
        )), Times.Once);
        repo.VerifyNoOtherCalls();
    }
}