using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Planner.Models.Blobs;
using Planner.Models.Repositories;
using Planner.Web.Controllers;
using Xunit;

namespace Planner.Web.Test.Controllers
{
    public class BlobContentControllerTest
    {
        private readonly Mock<IBlobContentStore> storage = new();
        private readonly Mock<IRemoteRepository<Blob>> repo = new();
        
        
        private readonly BlobContentController sut;

        public BlobContentControllerTest()
        {
            sut = new BlobContentController(repo.Object, storage.Object);
        }

        [Fact]
        public async Task ReadBlob()
        {
            var blob = new Blob() {Key = Guid.NewGuid(), MimeType = "image/png"};
            repo.Setup(i => i.ItemsFromKeys(It.Is<IEnumerable<Guid>>(i => i.Single() == blob.Key))).Returns(
                new[] {blob}.ToAsyncEnumerable());
            var stream = new MemoryStream();
            storage.Setup(i => i.Read(blob)).ReturnsAsync(stream);

            var ret = (FileStreamResult)(await sut.Get(blob.Key));
            Assert.Equal("image/png", ret.ContentType);
            Assert.Equal(stream, ret.FileStream);
        }

        [Fact]
        public async Task PutBlob()
        {
            var context = new DefaultHttpContext();
            var ms = new MemoryStream();
            context.Request.Body = ms;
            sut.ControllerContext = new ControllerContext(){HttpContext = context};
            var key = Guid.NewGuid();
            await sut.Put(key);
            
            storage.Verify(i=>i.Write(key, ms), Times.Once);
            storage.VerifyNoOtherCalls();
            repo.VerifyNoOtherCalls();
        }
    }
}