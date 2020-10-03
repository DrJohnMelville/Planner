using System;
using System.Threading.Tasks;
using Moq;
using NodaTime;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Planner.Models.Tasks;
using Xunit;

namespace Planner.Repository.Test.Cache
{
    public class CachedRepositoryTest2
    {
        private readonly Mock<ICachedRepositorySource<Note>> repo =
            new Mock<ICachedRepositorySource<Note>>();
        private readonly CachedRepository<Note> sut;
        private readonly LocalDate baseDate = new LocalDate(1975,7,28);

        public CachedRepositoryTest2()
        {
            sut = new CachedRepository<Note>(repo.Object);
        }

        [Fact]
        public async Task TestMethod()
        {
            var tcs = new TaskCompletionSource<int>();
            repo.Setup(i => i.ItemsForDate(baseDate)).Returns(
                (LocalDate d)=>NewList(tcs));
            var l1 = sut.ItemsForDate(baseDate);
            var t1 = sut.CompletedItemsForDate(baseDate);
            Assert.False(t1.IsCompleted);
            tcs.SetResult(1);
            var l2 = await t1;
            Assert.Equal(l1, l2);
        }

        private static ItemList<Note> NewList(TaskCompletionSource<int> tcs)
        {
            var ret = new ItemList<Note>();
            ret.SetCompletionTask(tcs.Task);
            return ret;
        }
    }
    public class CachedTaskRepositoryTest
    {
        private readonly TemporaryPTF source = new TemporaryPTF();
        private readonly CachedRepository<PlannerTask> sut;
        private readonly LocalDate baseDate = new LocalDate(1975,7,28);

        public CachedTaskRepositoryTest()
        {
            sut = new CachedRepository<PlannerTask>(source);
        }

        [Fact]
        public void ReturnCachedList()
        {
            Assert.Same(sut.ItemsForDate(baseDate), sut.ItemsForDate(baseDate));
        }
        [Fact]
        public void UniqueListPerDay()
        {
            Assert.NotSame(sut.ItemsForDate(baseDate.PlusDays(1)), sut.ItemsForDate(baseDate));
        }

        [Fact]
        public void AddTaskToProperDay()
        {
            var list = sut.ItemsForDate(baseDate);
            Assert.Equal(4, list.Count);
            sut.CreateTask("Foo", baseDate);
            Assert.Equal(5, list.Count);
        }

        [Fact]
        public void AbandonTaskUponGC()
        {
            CreateItemInSeparateMethod();
            GC.Collect(2, GCCollectionMode.Forced, true, true);
            GC.WaitForPendingFinalizers();
            GC.Collect(2, GCCollectionMode.Forced, true, true);
            Assert.Equal(4, sut.ItemsForDate(baseDate).Count);
        }

        private void CreateItemInSeparateMethod()
        {
            var list = sut.ItemsForDate(baseDate);
            sut.CreateTask("Foo", baseDate);
            Assert.Equal(5, list.Count);
        }
    }
}