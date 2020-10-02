using System;
using System.Collections.Generic;
using NodaTime;

namespace Planner.Models.Repositories
{
    public class CachedRepository<T>:ILocalRepository<T> where T:PlannerItemWithDate
    {
        private readonly Dictionary<LocalDate, WeakReference<IList<T>>> cache =
            new Dictionary<LocalDate, WeakReference<IList<T>>>();
        private readonly ILocalRepository<T> source;

        public CachedRepository(ILocalRepository<T> source)
        {
            this.source = source;
        }

        public T CreateTask(LocalDate date)
        {
            var list = TasksForDate(date); 
            // get the list before creating the task so we know that the new task is not in the list. 
            var ret = source.CreateTask(date);
            list.Add(ret);
            return ret;
        }

        public IList<T> TasksForDate(LocalDate date)
        {
            if (cache.TryGetValue(date, out var weakRef) && weakRef.TryGetTarget(out var val)) return val;
            var ret = source.TasksForDate(date);
            cache[date] = new WeakReference<IList<T>>(ret);
            return ret;
        }
    }
}