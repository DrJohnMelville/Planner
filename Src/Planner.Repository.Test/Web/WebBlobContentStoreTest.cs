using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Melville.TestHelpers.Http;
using Moq;
using Planner.Models.Blobs;
using Planner.Repository.Web;
using Xunit;

namespace Planner.Repository.Test.Web
{
    public class WebBlobContentStoreTest
    {
        private readonly Mock<IHttpClientMock> httpMock = new Mock<IHttpClientMock>();
        private readonly WebBlobContentStore sut;

        public WebBlobContentStoreTest()
        { 
            var httpClient = httpMock.ToHttpClient();
            httpClient.BaseAddress = new Uri("https://localhost/");
            sut = new WebBlobContentStore(httpClient);
        }

        [Fact]
        public async Task ReadTest()
        {
            var guid = Guid.NewGuid();
            httpMock.Setup(i=>i.EndsWith($"/BlobContent/{guid}"), HttpMethod.Get)
                .ReturnsHttp("Hello", "image/png");
            var ret = await sut.Read(new Blob() {Key = guid});
            Assert.Equal("Hello", await new StreamReader(ret).ReadToEndAsync());
        }

        [Fact]
        public async Task WriteTest()
        {
            var str = new MemoryStream();
            var guid = Guid.NewGuid();
            httpMock.Setup(i=>i.EndsWith($"/BlobContent/{guid}"), HttpMethod.Put)
                .ReturnsHttp("x", "text/plain");

            await sut.Write(guid, str);
            
            httpMock.Verify(
                (Func<string, bool>)(i=>i.EndsWith($"/BlobContent/{guid}")),
                HttpMethod.Put, Times.Once);
        }

    }
}