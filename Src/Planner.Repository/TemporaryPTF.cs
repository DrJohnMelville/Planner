using System;
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
        public PlannerTask CreateItem( LocalDate date, Action<PlannerTask> initialize)
        {
            var ret = new PlannerTask();
            initialize(ret);
            return ret;
        }

        public IListPendingCompletion<PlannerTask> ItemsForDate(LocalDate date)
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