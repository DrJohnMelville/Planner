using Melville.SystemInterface.Time;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Planner.Models.Blobs;
using Planner.Models.Repositories;
using Planner.Models.Time;
using Planner.Web.Controllers;

namespace TUnit.Web;

public class BlobContentControllerTest
{
    private readonly Mock<IUsersClock> clock = new();
    private readonly Mock<IBlobContentStore> storage = new();
    private readonly Mock<IDatedRemoteRepository<Blob>> repo = new();
    private readonly BlobContentController sut = new BlobContentController();

    public BlobContentControllerTest()
    {
        clock.Setup(i => i.CurrentUiTimeZone()).Returns(DateTimeZone.Utc);
    }

    private BlobStreamExtractor CreateExtractor() =>
        new(new LocalToRemoteRepositoryBridge<Blob>(repo.Object, Mock.Of<IWallClock>(), clock.Object), storage.Object);

    [Test]
    public async Task ReadBlob()
    {
        var blob = new Blob() {Key = Guid.NewGuid(), MimeType = "image/png"};
        repo.Setup(i => i.ItemsFromKeys(It.Is<IEnumerable<Guid>>(i => i.Single() == blob.Key))).Returns(
            new[] {blob}.ToAsyncEnumerable());
        var stream = new MemoryStream();
        storage.Setup(i => i.Read(blob)).ReturnsAsync(stream);

        var ret = (FileStreamResult)(await sut.Get(blob.Key, 
            CreateExtractor()));
        Assert.Equal("image/png", ret.ContentType);
        Assert.Equal(stream, ret.FileStream);
    }
    [Test]
    public async Task ReadBlobByPosition()
    {
        var blob = new Blob() {Key = Guid.NewGuid(), MimeType = "image/png"};
        repo.Setup(i => i.TasksForDate(new LocalDate(1975,7,1), DateTimeZone.Utc)).Returns(
            new[] {blob}.ToAsyncEnumerable());
        var stream = new MemoryStream();
        storage.Setup(i => i.Read(blob)).ReturnsAsync(stream);

        var ret = (FileStreamResult)(await sut.Get(new LocalDate(1975,07,28), "7.1_1", 
            CreateExtractor()));
        Assert.Equal("image/png", ret.ContentType);
        Assert.Equal(stream, ret.FileStream);
    }

    [Test]
    public async Task PutBlob()
    {
        var context = new DefaultHttpContext();
        var ms = new MemoryStream();
        context.Request.Body = ms;
        sut.ControllerContext = new ControllerContext(){HttpContext = context};
        var key = Guid.NewGuid();
        await sut.Put(key, storage.Object);
            
        storage.Verify(i=>i.Write(key, ms), Times.Once);
        storage.VerifyNoOtherCalls();
        repo.VerifyNoOtherCalls();
    }
}