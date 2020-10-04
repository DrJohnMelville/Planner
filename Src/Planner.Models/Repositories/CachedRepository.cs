using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;

namespace Planner.Models.Repositories
{
    public interface ICachedRepositorySource<T> : ILocalRepository<T> where T : PlannerItemWithDate
    {
    }

    public class CachedRepository<T> : ILocalRepository<T> where T : PlannerItemWithDate
    {
        private readonly Dictionary<LocalDate, WeakReference<IList<T>>> cache =
            new Dictionary<LocalDate, WeakReference<IList<T>>>();

        private readonly ILocalRepository<T> source;

        public CachedRepository(ICachedRepositorySource<T> source)
        {
            this.source = source;
        }

        public T CreateItem(LocalDate date, Action<T> initialize)
        {
            var list = ItemsForDate(date);
            // get the list before creating the task so we know that the new task is not in the list. 
            var ret = source.CreateItem(date, initialize);
            list.Add(ret);
            return ret;
        }

        public IList<T> ItemsForDate(LocalDate date)
        {
            if (cache.TryGetValue(date, out var weakRef) && weakRef.TryGetTarget(out var val)) return val;
            var ret = source.ItemsForDate(date);
            cache[date] = new WeakReference<IList<T>>(ret);
            return ret;
        }

        public Task<IList<T>> CompletedItemsForDate(LocalDate date)
        {
            var ret = ItemsForDate(date);
            return  ret is IListPendingCompletion<T> lpc?
                lpc.CompleteList(): Task.FromResult(ret);
        }
    }
}