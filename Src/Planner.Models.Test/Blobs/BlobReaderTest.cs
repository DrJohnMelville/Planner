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
        private readonly BlobGenerator sut;
        private readonly LocalDate date = new LocalDate(1975,07,28);
        private readonly Mock<ILocalRepository<Blob>> repo = new Mock<ILocalRepository<Blob>>();
        private readonly Mock<IBlobContentStore> store = new Mock<IBlobContentStore>();
        private readonly ItemList<Blob> todaysBlobs = new ItemList<Blob>(); 
        
        public BlobReaderTest()
        {
            repo.Setup(i => i.ItemsForDate(date)).Returns(todaysBlobs);
            repo.Setup(i => i.ItemsForDate(date.PlusDays(1))).Returns(new ItemList<Blob>());
            sut = new BlobGenerator(repo.Object, store.Object);
        }

        [Theory]
        [InlineData("1975-07-28/7.28_1000", 0)]
        [InlineData("1975-07-28/7.28_0", 0)]
        [InlineData("1975-07-28/7.28_2", 0)]
        [InlineData("1975-07-28/7.28_1", 3)]
        [InlineData("1975-07-28/7.28.75_1", 3)]
        [InlineData("1975-07-28/7.28.1975_1", 3)]
        [InlineData("1975-07-28/7.29_1", 0)]
        public async Task NonExistantPropertyReturnsEmpty(string url, int len)
        {
            var dest = new MemoryStream();
            var item = new Blob();
            todaysBlobs.Add(item);
            var resultStr = new MemoryStream(new byte[]{1,2,3});
            store.Setup(i => i.Read(item)).ReturnsAsync(resultStr);
            await sut.TryRespond(url,dest)!;
            Assert.Equal(len, dest.Length);
        }
    }
}