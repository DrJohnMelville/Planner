using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Moq;
using NodaTime;
using Planner.Models.Blobs;
using Planner.Models.Repositories;
using Xunit;

namespace Planner.Models.Test.Blobs
{
    public class BlobCreatorTest
    {
        private readonly Mock<ILocalRepository<Blob>> repo = new Mock<ILocalRepository<Blob>>();
        private readonly ItemList<Blob> dailyList = new ItemList<Blob>();
        private readonly Mock<IClock> clock = new Mock<IClock>();
        private readonly Mock<IBlobContentStore> writer = new Mock<IBlobContentStore>();
        private readonly BlobCreator sut;
        private readonly LocalDate date = new LocalDate(1975,07,28);

        public BlobCreatorTest()
        {
            repo.Setup(i => i.ItemsForDate(date)).Returns(dailyList);
            repo.Setup(i => i.CreateItem(date, It.IsAny<Action<Blob>>()))
                .Returns((LocalDate date, Action<Blob> init) =>
                {
                    var ret = new Blob() {Date = date};
                    init(ret);
                    dailyList.Add(ret);
                    return ret;
                });
            sut = new BlobCreator(repo.Object, clock.Object, writer.Object);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(234)]
        public async Task CreateImage(int priorImages)
        {
            for (int i = 0; i < priorImages; i++)
            {
                dailyList.Add(new Blob());
            }

            var memoryStream = new MemoryStream();
            Assert.Equal($"![Pasted Image](7.28_{priorImages+1})", 
                await sut.MarkdownForNewImage("Pasted Image", "image/png", date, memoryStream));
            writer.Verify(i=>i.Write(It.Is<Blob>(b=>
                b.Key!= Guid.Empty && 
                b.Name == "Pasted Image" &&
                b.MimeType == "image/png" &&
                b.Date == date), memoryStream), Times.Once);
        }
    }
}