﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.HtmlGeneration;

namespace Planner.Models.Repositories
{
    public class ClearCachesEventArgs: EventArgs{}
    
    public interface ICachedRepositorySource<T> : ILocalRepository<T> where T : PlannerItemWithDate
    {
    }

    public class CachedRepository<T> : ILocalRepository<T> where T : PlannerItemWithDate
    {
        private readonly Dictionary<LocalDate, WeakReference<IListPendingCompletion<T>>> cache =
            new();

        private readonly ILocalRepository<T> source;

        public CachedRepository(ICachedRepositorySource<T> source, 
            IEventBroadcast<ClearCachesEventArgs> clearCacheSignal)
        {
            this.source = source;
            clearCacheSignal.Fired += ClearCache;
        }

        private void ClearCache(object? sender, ClearCachesEventArgs e) => cache.Clear();

        public T CreateItem(LocalDate date, Action<T> initialize)
        {
            var list = ItemsForDate(date);
            // get the list before creating the task so we know that the new task is not in the list. 
            var ret = source.CreateItem(date, initialize);
            list.Add(ret);
            return ret;
        }

        public IListPendingCompletion<T> ItemsForDate(LocalDate date)
        {
            if (cache.TryGetValue(date, out var weakRef) && weakRef.TryGetTarget(out var val)) return val;
            var ret = source.ItemsForDate(date);
            cache[date] = new WeakReference<IListPendingCompletion<T>>(ret);
            return ret;
        }
    }
}