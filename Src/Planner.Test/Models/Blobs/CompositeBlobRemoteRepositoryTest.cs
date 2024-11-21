using NodaTime;
using Planner.Models.Blobs;
using Planner.Models.Repositories;

namespace Planner.Test.Models.Blobs;

public class CompositeBlobRemoteRepositoryTest
{
    private readonly Mock<IDatedRemoteRepository<Blob>> repo = new Mock<IDatedRemoteRepository<Blob>>();
    private readonly Mock<IDeletableBlobContentStore> store = new Mock<IDeletableBlobContentStore>();
    private readonly CompopsiteBlobRemoteRepository sut;
    private readonly Blob blob = new Blob() {Name = "Name", Key = Guid.NewGuid()};
    private readonly DateTimeZone timeZone = DateTimeZone.Utc;

    public CompositeBlobRemoteRepositoryTest()
    {
        sut = new CompopsiteBlobRemoteRepository(repo.Object, store.Object);
    }

    [Test]
    public async Task AddBlob()
    {
        await sut.Add(blob);
        repo.Verify(i=>i.Add(blob), Times.Once);
        repo.VerifyNoOtherCalls();
        store.VerifyNoOtherCalls();
    }
    [Test]
    public async Task UpdateBlob()
    {
        await sut.Update(blob);
        repo.Verify(i=>i.Update(blob), Times.Once);
        repo.VerifyNoOtherCalls();
        store.VerifyNoOtherCalls();
    }
    [Test]
    public async Task DeleteBlob()
    {
        await sut.Delete(blob);
        repo.Verify(i=>i.Delete(blob), Times.Once);
        store.Verify(i=>i.Delete(blob), Times.Once);
        repo.VerifyNoOtherCalls();
        store.VerifyNoOtherCalls();
    }

    [Test]
    public void TasksForDateTest()
    {
        var ret = Mock.Of<IAsyncEnumerable<Blob>>();
        var date = new LocalDate(1975, 7, 28);
        repo.Setup(i => i.TasksForDate(date, timeZone)).Returns(ret);
        Assert.Equal(ret, sut.TasksForDate(date, timeZone));
    }


}