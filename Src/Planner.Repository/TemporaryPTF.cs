using System.Collections.Generic;
using System.Threading.Tasks;
using Melville.MVVM.AdvancedLists;
using NodaTime;
using Planner.Models.Repositories;
using Planner.Models.Tasks;

namespace Planner.Repository
{
    public class TemporaryPTF : ICachedRepositorySource<PlannerTask>
    {
        public PlannerTask CreateTask( LocalDate date)
        {
            return new PlannerTask();
        }

        public IList<PlannerTask> ItemsForDate(LocalDate date)
        {
            var src = new ThreadSafeBindableCollection<PlannerTask>();
            src.Add(new PlannerTask() {Name = "Task1"});
            src.Add(new PlannerTask() {Name = "Task2"});
            src.Add(new PlannerTask() {Name = "Task3"});
            src.Add(new PlannerTask() {Name = "Task4"});
            return src;
        }

        public Task<IList<PlannerTask>> CompletedItemsForDate(LocalDate date) => 
          Task.FromResult(ItemsForDate(date));
    }
}