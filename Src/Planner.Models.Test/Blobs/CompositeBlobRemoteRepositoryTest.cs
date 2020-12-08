﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NodaTime;
using Planner.Models.Blobs;
using Planner.Models.Repositories;
using Xunit;

namespace Planner.Models.Test.Blobs
{
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

        [Fact]
        public async Task AddBlob()
        {
            await sut.Add(blob);
            repo.Verify(i=>i.Add(blob), Times.Once);
            repo.VerifyNoOtherCalls();
            store.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task UpdateBlob()
        {
            await sut.Update(blob);
            repo.Verify(i=>i.Update(blob), Times.Once);
            repo.VerifyNoOtherCalls();
            store.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task DeleteBlob()
        {
            await sut.Delete(blob);
            repo.Verify(i=>i.Delete(blob), Times.Once);
            store.Verify(i=>i.Delete(blob), Times.Once);
            repo.VerifyNoOtherCalls();
            store.VerifyNoOtherCalls();
        }

        [Fact]
        public void TasksForDateTest()
        {
            var ret = Mock.Of<IAsyncEnumerable<Blob>>();
            var date = new LocalDate(1975, 7, 28);
            repo.Setup(i => i.TasksForDate(date, timeZone)).Returns(ret);
            Assert.Equal(ret, sut.TasksForDate(date, timeZone));
        }


    }
}