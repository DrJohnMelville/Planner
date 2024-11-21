using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Repositories;
using Planner.Models.Tasks;

namespace TUnit.Repository.Cache;

    public class CachedRepoClearsWithGarbageCollection
    {
        private readonly TemporaryPTF source = new();
        private readonly CachedRepository<PlannerTask> sut;
        private readonly LocalDate baseDate = new(1975, 7, 28);
        private readonly EventBroadcast<ClearCachesEventArgs> message = new();

        public CachedRepoClearsWithGarbageCollection()
        {
            sut = new CachedRepository<PlannerTask>(source, message);
        }

        [Test]
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
        
        public class TemporaryPTF : ICachedRepositorySource<PlannerTask>
        {
            public PlannerTask CreateItem( LocalDate date, Action<PlannerTask> initialize)
            {
                var ret = new PlannerTask();
                initialize(ret);
                return ret;
            }

            public IListPendingCompletion<PlannerTask> ItemsForDate(LocalDate date)
            {
                return new ItemList<PlannerTask>
                {
                    new () {Name = "Task1"},
                    new () {Name = "Task2"},
                    new () {Name = "Task3"},
                    new () {Name = "Task4"}
                };
            }

            public IListPendingCompletion<PlannerTask> ItemsByKeys(IEnumerable<Guid> keys)
            {
                var src = new ItemList<PlannerTask>();
                src.Add(new PlannerTask() {Name = "Task1"});
                src.Add(new PlannerTask() {Name = "Task2"});
                src.Add(new PlannerTask() {Name = "Task3"});
                src.Add(new PlannerTask() {Name = "Task4"});
                return src;
            }
        }
    }