using NodaTime;
using Planner.Models.Tasks;
using Planner.Repository.SqLite;

namespace TUnit.Repository.SqLite;

public class SqlPlanerTaskRepositoryTest
{
    private readonly TestDatabase data = new TestDatabase();
    private readonly SqlRemoteRepositoryWithDate<PlannerTask> sut;
    private readonly LocalDate date1 = new LocalDate(1975,07,28);
    private readonly LocalDate date2 = new LocalDate(1974,08,18);

    public SqlPlanerTaskRepositoryTest()
    {
        sut = new SqlRemoteRepositoryWithDate<PlannerTask>(data.NewContext);
    }

    [Test]
    public async Task CreatingATaskAddsItToTheListForTheDay()
    {
        var pt = new PlannerTask(Guid.NewGuid()) {Name = "Foo", Date = date1};
        await sut.Add(pt);
        var list = await sut.TasksForDate(date1, DateTimeZone.Utc).ToListAsync();
        Assert.Single(list);
        Assert.Equal("Foo", list[0].Name);
    }
    [Test]
    public async Task ModificationsGetSaved()
    {
        var pt = new PlannerTask(Guid.NewGuid()) {Name = "Foo", Date = date1};
        await sut.Add(pt);
        pt.Name = "Bar";
        await sut.Update(pt);
        var list = await sut.TasksForDate(date1, DateTimeZone.Utc).ToListAsync();
        Assert.Single(list);
        Assert.Equal("Bar", list[0].Name);
    }
    [Test]
    public async Task TasksOnlyQueryFromTheIndicatedDate()
    {
        var pt = new PlannerTask(Guid.NewGuid()) {Name = "Foo", Date = date1};
        await sut.Add(pt);
        var list = await sut.TasksForDate(date2, DateTimeZone.Utc).ToListAsync();
        Assert.Empty(list);
    }
    [Test]
    public async Task DeleteTask()
    {
        var pt = new PlannerTask(Guid.NewGuid()) {Name = "Foo", Date = date1};
        await sut.Add(pt);
        var list = await sut.TasksForDate(date1, DateTimeZone.Utc).ToListAsync();
        Assert.Single(list);
        await sut.Delete(pt);
        Assert.Empty(await sut.TasksForDate(date1,DateTimeZone.Utc).ToListAsync());
    }
}