using System;
using NodaTime;
using Planner.Models.Tasks;
using Xunit;

namespace Planner.Repository.Test.Cache
{
    public class CachedTaskRepositoryTest
    {
        private readonly TemporaryPTF source = new TemporaryPTF();
        private readonly CachedTaskRepository sut;
        private readonly LocalDate baseDate = new LocalDate(1975,7,28);

        public CachedTaskRepositoryTest()
        {
            sut = new CachedTaskRepository(source);
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
            var list = sut.TasksForDate(baseDate);
            sut.CreateTask("Foo", baseDate);
            Assert.Equal(5, list.Count);
            list = new PlannerTaskList();
            list.Add(new PlannerTask());
            GC.Collect(2, GCCollectionMode.Forced, true, true);
            GC.WaitForPendingFinalizers();
            GC.Collect(2, GCCollectionMode.Forced, true, true);
            list = sut.TasksForDate(baseDate);
            Assert.Equal(4, list.Count);
        }
    }
}