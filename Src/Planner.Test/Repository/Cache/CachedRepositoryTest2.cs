using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Notes;
using Planner.Models.Repositories;

namespace TUnit.Repository.Cache;

public class CachedRepositoryTest2
{
    private readonly Mock<ICachedRepositorySource<Note>> repo =
        new();

    private readonly CachedRepository<Note> sut;
    private readonly LocalDate baseDate = new(1975, 7, 28);
    private readonly EventBroadcast<ClearCachesEventArgs> message = new();
    private readonly TaskCompletionSource<int> tcs = new();
    private readonly Guid k1 = Guid.NewGuid();
    private readonly Guid k2 = Guid.NewGuid();

    public CachedRepositoryTest2()
    {
        sut = new CachedRepository<Note>(repo.Object, message);
        repo.Setup(i => i.ItemsForDate(It.IsAny<LocalDate>())).Returns((LocalDate _) =>
        {
            var ret = new ItemList<Note>() {new(){Key =k1 }, new(){Key=k2}};
            ret.SetCompletionTask(tcs.Task);
            return ret;
        });
        repo.Setup(i => i.ItemsByKeys(It.IsAny<IEnumerable<Guid>>())).Returns((IEnumerable<Guid> keys) =>
        {
            var ret = new ItemList<Note>();
            foreach (var key in keys)
            {
                ret.Add(new Note(){Key = key});
            }
            ret.SetCompletionTask(tcs.Task);
            return ret;
        });
    }

    [Test]
    public async Task SearchingForCachedItemDoesNotHitDatabase()
    {
        tcs.SetResult(1);
        var date = await sut.ItemsForDate(baseDate).CompleteList();
        var byKey = await sut.ItemsByKeys(new[] {date[0].Key}).CompleteList();
        Assert.Equal(byKey[0], date[0]);
        repo.Verify(i=>i.ItemsForDate(baseDate), Times.Once);
        repo.VerifyNoOtherCalls();
    }
        
    [Test]
    public async Task DuplicateKeyQueryReturnsSameObject()
    {
        tcs.SetResult(1);
        Assert.Equal((await sut.ItemsByKeys(new []{k1, k2}).CompleteList()).First(),
            (await sut.ItemsByKeys(new []{k1, k2}).CompleteList()).First());
    }
    [Test]
    public async Task SameObjectTwoWays()
    {
        tcs.SetResult(1);
        Assert.Equal((await sut.ItemsByKeys(new []{k1, Guid.NewGuid()}).CompleteList()).First(),
            (await sut.ItemsForDate(baseDate).CompleteList()).First());
    }
    [Test]
    public async Task SameObjectReverse()
    {
        tcs.SetResult(1);
        var byDate = (await sut.ItemsForDate(baseDate).CompleteList());
        var byIndex = await sut.ItemsByKeys(new []{k1, Guid.NewGuid()}).CompleteList();
        Assert.Equal(byDate.First(), byIndex.First());
        Assert.Equal(2, byIndex.Count);
            
    }
    [Test]
    public async Task NewKeyQueryReturnsDifferentObject()
    {
        tcs.SetResult(1);
        Assert.NotEqual((await sut.ItemsByKeys(new []{k1, Guid.NewGuid()}).CompleteList()).Last(),
            (await sut.ItemsByKeys(new []{k1, Guid.NewGuid()}).CompleteList()).Last());
    }


    [Test]
    public async Task AwaitForCompletionOnTheReturnedList()
    {
        var l1 = sut.ItemsForDate(baseDate);
        var t1 = sut.ItemsForDate(baseDate).CompleteList();
        Assert.False(t1.IsCompleted);
        tcs.SetResult(1);
        var l2 = await t1;
        Assert.Equal(l1, l2);
    }

    [Test]
    public void ReturnCachedList()
    {
        Assert.Same(sut.ItemsForDate(baseDate), sut.ItemsForDate(baseDate));
    }

    [Test]
    public void ClearCaches()
    {
        var preClearItem = sut.ItemsForDate(baseDate);
        message.Fire(this, new ClearCachesEventArgs());
        var postCleatItem = sut.ItemsForDate(baseDate);
        Assert.NotSame(preClearItem, postCleatItem);
    }

    [Test]
    public void UniqueListPerDay()
    {
        Assert.NotSame(sut.ItemsForDate(baseDate.PlusDays(1)), sut.ItemsForDate(baseDate));
    }

    [Test]
    public void AddTaskToProperDay()
    {
        var list = sut.ItemsForDate(baseDate);
        Assert.Equal(2, list.Count);
        sut.CreateItem(baseDate, note => note.Title = "Hello");
        Assert.Equal(3, list.Count);
    }
}