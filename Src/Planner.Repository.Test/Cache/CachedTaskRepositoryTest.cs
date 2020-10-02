using System;
using NodaTime;
using Planner.Models.Repositories;
using Planner.Models.Tasks;
using Xunit;

namespace Planner.Repository.Test.Cache
{
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
            Assert.Same(sut.TasksForDate(baseDate), sut.TasksForDate(baseDate));
        }
        [Fact]
        public void UniqueListPerDay()
        {
            Assert.NotSame(sut.TasksForDate(baseDate.PlusDays(1)), sut.TasksForDate(baseDate));
        }

        [Fact]
        public void AddTaskToProperDay()
        {
            var list = sut.TasksForDate(baseDate);
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
            Assert.Equal(4, sut.TasksForDate(baseDate).Count);
        }

        private void CreateItemInSeparateMethod()
        {
            var list = sut.TasksForDate(baseDate);
            sut.CreateTask("Foo", baseDate);
            Assert.Equal(5, list.Count);
        }
    }
}