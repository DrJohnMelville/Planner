using System;
using System.Collections.Generic;
using NodaTime;
using Planner.Models.Repositories;
using Planner.Models.Tasks;

namespace Planner.Repository
{
    public class CachedTaskRepository:IPlannerTaskRepository
    {
        private readonly Dictionary<LocalDate, WeakReference<PlannerTaskList>> cache =
            new Dictionary<LocalDate, WeakReference<PlannerTaskList>>();
        private readonly IPlannerTaskRepository source;

        public CachedTaskRepository(IPlannerTaskRepository source)
        {
            this.source = source;
        }

        public PlannerTask CreateTask(string title, LocalDate date)
        {
            var list = TasksForDate(date); 
              // get the list before creating the task so we know that the new task is not in the list. 
            var ret = source.CreateTask(title, date);
            list.Add(ret);
            return ret;
        }

        public PlannerTaskList TasksForDate(LocalDate date)
        {
            if (cache.TryGetValue(date, out var weakRef) && weakRef.TryGetTarget(out var val)) return val;
            var ret = source.TasksForDate(date);
            cache[date] = new WeakReference<PlannerTaskList>(ret);
            return ret;
        }
    }
}