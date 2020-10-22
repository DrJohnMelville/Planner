using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Moq;
using NodaTime;
using Planner.Models.Blobs;
using Planner.Models.Repositories;
using Xunit;

namespace Planner.Models.Test.Blobs
{
    public class BlobReaderTest
    {
        private readonly BlobReader sut;
        private readonly LocalDate date = new LocalDate(1975,07,28);
        private readonly Mock<ILocalRepository<Blob>> repo = new Mock<ILocalRepository<Blob>>();
        private readonly Mock<IBlobContentStore> store = new Mock<IBlobContentStore>();
        private readonly List<Blob> todaysBlobs = new List<Blob>(); 
        
        public BlobReaderTest()
        {
            repo.Setup(i => i.CompletedItemsForDate(date)).ReturnsAsync(todaysBlobs);
            sut = new BlobReader(repo.Object, store.Object);
        }

        [Fact]
        public async Task NonExistantPropertyReturnsEmpty()
        {
            var str = await sut.Read(date, 1000);
            Assert.Equal(0, str.Length);
        }

        [Fact]
        public async Task GetNamedStream()
        {
            var item = new Blob();
            todaysBlobs.Add(item);
            var resultStr = new MemoryStream(new byte[]{1,2,3});
            store.Setup(i => i.Read(item)).ReturnsAsync(resultStr);

            Assert.Equal(resultStr, await sut.Read(date, 1));
            
        }

    }
}