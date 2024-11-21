using Melville.SystemInterface.Time;
using NodaTime;
using Planner.Models.Repositories;
using Planner.Models.Tasks;
using Planner.Models.Time;

namespace Planner.Test.Models.Repositories;

public class PlannerTskLocalToRemoteRepositoryBridgeTest
{
    private readonly Mock<IDatedRemoteRepository<PlannerTask>> repo =
        new Mock<IDatedRemoteRepository<PlannerTask>>();

    private readonly Mock<IUsersClock> usersClock = new();
    private readonly Mock<IWallClock> clock = new();
    private readonly LocalDate date = new(1975, 07, 28);

    public LocalToRemoteRepositoryBridge<PlannerTask> sut;

    public PlannerTskLocalToRemoteRepositoryBridgeTest()
    {
        usersClock.Setup(i => i.CurrentUiTimeZone()).Returns(DateTimeZone.Utc);
        sut = new LocalToRemoteRepositoryBridge<PlannerTask>(repo.Object, clock.Object, usersClock.Object);
    }

    [Test]
    public void CreateTaskUpdatesRemoteRepository()
    {
        var task = sut.CreateTask("Foo", date);
        repo.Verify(i => i.Add((PlannerTask)task), Times.Once);
    }

    [Test]
    public void ModifyFieldUpdatesTask()
    {
        var task = sut.CreateTask("Foo", date);
        task.Name = "Bar";
        repo.Verify(i => i.Add((PlannerTask)task), Times.Once);
        repo.Verify(i => i.Update((PlannerTask)task), Times.Once);
    }

    [Test]
    public void CollapseMultipleUpdates()
    {
        var tcs = new TaskCompletionSource<int>();
        clock.Setup(I => I.Wait(It.IsAny<TimeSpan>())).Returns(tcs.Task);
        var task = sut.CreateTask("Foo", date);
        task.Name = "Bar";
        task.Priority = 'A';
        task.Order = 1;
        tcs.SetResult(1);
        repo.Verify(i => i.Add((PlannerTask)task), Times.Once);
        repo.Verify(i => i.Update((PlannerTask)task), Times.Once);
    }

    private async IAsyncEnumerable<T> AsyncEnum<T>(Task delay, params T[] items)
    {
        await (delay ?? Task.CompletedTask);
        foreach (var item in items)
        {
            yield return item;
        }
    }

    [Test]
    public void ChangingALoadedTaskCausesAnUpdate()
    {
        var task = new PlannerTask(Guid.NewGuid());
        repo.Setup(i => i.TasksForDate(date, DateTimeZone.Utc)).Returns(AsyncEnum(null!, task));
        var list = sut.ItemsForDate(date);
        Assert.Single(list);
        list[0].Name = "Bar";
        repo.Verify(i => i.Update((PlannerTask)list[0]), Times.Once);
    }

    [Test]
    public void RemovingTaskDeletesFromDatabase()
    {
        var task = new PlannerTask(Guid.NewGuid());
        repo.Setup(i => i.TasksForDate(date, DateTimeZone.Utc)).Returns(AsyncEnum(null!, task));
        var list = sut.ItemsForDate(date);
        Assert.Single(list);
        var item = list[0];
        list.RemoveAt(0);
        repo.Verify(i => i.Delete((PlannerTask)item), Times.Once);
    }

    [Test]
    public async Task DelayedTaskLoading()
    {
        var tcs = new TaskCompletionSource<int>();
        var task = new PlannerTask(Guid.NewGuid());
        repo.Setup(i => i.TasksForDate(date, DateTimeZone.Utc)).Returns(AsyncEnum(tcs.Task, task));
        var list = sut.ItemsForDate(date);
        Assert.Empty(list);
        tcs.SetResult(1);
        await ((IListPendingCompletion<PlannerTask>)list).CompleteList();
        Assert.Single(list);
    }

    [Test]
    public async Task LoadByKeys()
    {
        var tcs = new TaskCompletionSource<int>();
        var newGuid = Guid.NewGuid();
        var query = new[] { newGuid };
        var task = new PlannerTask(newGuid);
        repo.Setup(i => i.ItemsFromKeys(query)).Returns(AsyncEnum(tcs.Task, task));
        var list = sut.ItemsByKeys(query);
        Assert.Empty(list);
        tcs.SetResult(1);
        await ((IListPendingCompletion<PlannerTask>)list).CompleteList();
        Assert.Single(list);
    }
}