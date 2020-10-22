using System;
using System.IO;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using Melville.Mvvm.TestHelpers.MockFiles;
using Planner.Models.Blobs;
using Xunit;

namespace Planner.Models.Test.Blobs
{
    public class BlobContentStoreTest
    {
        private MockDirectory dir = new MockDirectory("c:\\sss");
        private BlobContentStore sut;

        public BlobContentStoreTest()
        {
            sut = new BlobContentStore(dir);
        }

        private async Task<Blob> WriteHelloBlob()
        {
            await using var ms = new MemoryStream(Encoding.UTF8.GetBytes("Hello"));
            var blob = new Blob() {Key = Guid.NewGuid()};
            await sut.Write(blob, ms);
            return blob;
        }

        [Fact]
        public async Task WriteBlob()
        {
            var blob = await WriteHelloBlob();
            Assert.Equal(5, dir.File(blob.Key.ToString()).Size);
        }

        [Fact]
        public async Task RoundTrip()
        {
            var blob = await WriteHelloBlob();
            
            using var tr = new StreamReader(await  sut.Read(blob));
            Assert.Equal("Hello", await tr.ReadToEndAsync());
            
        }

    }
}