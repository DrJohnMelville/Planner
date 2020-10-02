using System;
using System.Collections.Generic;
using NodaTime;
using Planner.Models.Tasks;

namespace Planner.Models.Repositories
{
    public class CachedTaskRepository:ILocalPlannerTaskRepository
    {
        private readonly Dictionary<LocalDate, WeakReference<IList<PlannerTask>>> cache =
            new Dictionary<LocalDate, WeakReference<IList<PlannerTask>>>();
        private readonly ILocalPlannerTaskRepository source;

        public CachedTaskRepository(ILocalPlannerTaskRepository source)
        {
            this.source = source;
        }

        public PlannerTask CreateTask(string name, LocalDate date)
        {
            var list = TasksForDate(date); 
              // get the list before creating the task so we know that the new task is not in the list. 
            var ret = source.CreateTask(name, date);
            list.Add(ret);
            return ret;
        }

        public IList<PlannerTask> TasksForDate(LocalDate date)
        {
            if (cache.TryGetValue(date, out var weakRef) && weakRef.TryGetTarget(out var val)) return val;
            var ret = source.TasksForDate(date);
            cache[date] = new WeakReference<IList<PlannerTask>>(ret);
            return ret;
        }
    }
}