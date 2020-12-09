using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Melville.MVVM.Functional;
using NodaTime;
using Planner.Models.HtmlGeneration;

namespace Planner.Models.Repositories
{
    public class ClearCachesEventArgs: EventArgs{}
    
    public interface ICachedRepositorySource<T> : ILocalRepository<T> where T : class
    {
    }
    
    public class CachedRepository<T> : ILocalRepository<T> where T : PlannerItemWithDate
    {
        private readonly Dictionary<LocalDate, WeakReference<IListPendingCompletion<T>>> listCache =
            new();

        private readonly Dictionary<Guid, WeakReference<T>> itemCache = new();

        private readonly ILocalRepository<T> source;

        public CachedRepository(ICachedRepositorySource<T> source, 
            IEventBroadcast<ClearCachesEventArgs> clearCacheSignal)
        {
            this.source = source;
            clearCacheSignal.Fired += ClearCache;
        }

        private void ClearCache(object? sender, ClearCachesEventArgs e)
        {
            listCache.Clear();
            itemCache.Clear();
        }

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
            if (listCache.TryGetValue(date, out var weakRef) && weakRef.TryGetTarget(out var val)) return val;
            var ret = source.ItemsForDate(date);
            RegisterListForReconciliation(ret);
            listCache[date] = new WeakReference<IListPendingCompletion<T>>(ret);
            return ret;
        }
        
        public IListPendingCompletion<T> ItemsByKeys(IEnumerable<Guid> keys)
        {
            var ret = CheckCacheForItems(keys);
            RegisterListForReconciliation(ret);
            return ret;
        }
        
        private void RegisterListForReconciliation(IListPendingCompletion<T> ret) => 
            ret.SetCompletionTask(ret.CompleteList().ContinueWith(ReconcileList, null));

        private void ReconcileList(Task<IListPendingCompletion<T>> completingTask, object? arg2)
        {
            var list = completingTask.Result;
            ReconcileList(list);
        }

        private void ReconcileList(IListPendingCompletion<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (itemCache.TryGetValue(list[i].Key, out var weakRef))
                {
                    if (weakRef.TryGetTarget(out var item))
                    {
                        list[i] = item;
                    }
                    else
                    {
                        weakRef.SetTarget(list[i]);
                    }
                }
                else
                {
                    itemCache[list[i].Key] = new WeakReference<T>(list[i]);
                }
            }
        }

        private IListPendingCompletion<T> CheckCacheForItems(IEnumerable<Guid> keys)
        {
            var (remoteKeysToLoad, locallyFoundItems) = SortKeysIntoLocalAndRemote(keys);
            return AddKeysToList(remoteKeysToLoad, locallyFoundItems);
        }

        private (List<Guid> remoteKeysToLoad, ItemList<T> locallyFoundItems) SortKeysIntoLocalAndRemote(
            IEnumerable<Guid> keys)
        {
            var remoteKeysToLoad = new List<Guid>();
            var locallyFoundItems = new ItemList<T>();
            foreach (var key in keys)
            {
                SortSingleKey(key, locallyFoundItems, remoteKeysToLoad);
            }

            return (remoteKeysToLoad, locallyFoundItems);
        }

        private void SortSingleKey(Guid key, ItemList<T> locallyFoundItems, List<Guid> remoteKeysToLoad)
        {
            if (itemCache.TryGetValue(key, out var weakref) &&
                weakref.TryGetTarget(out var item))
            {
                locallyFoundItems.Add(item);
            }
            else
            {
                remoteKeysToLoad.Add(key);
            }
        }

        private IListPendingCompletion<T> AddKeysToList(List<Guid> remoteKeysToLoad, ItemList<T> ret) =>
            (remoteKeysToLoad.Count, ret.Count) switch
            {
                (0, _) => ret,
                (_, 0) => SublistByKeyQuery(remoteKeysToLoad),
                _ => CombineLists(ret, SublistByKeyQuery(remoteKeysToLoad))
            };

        private IListPendingCompletion<T> CombineLists(IListPendingCompletion<T> localItems, 
            IListPendingCompletion<T> remoteItems)
        {
            localItems.SetCompletionTask(remoteItems.CompleteList().ContinueWith(CopyRemoteToLocal));
            return localItems;

            void CopyRemoteToLocal(Task<IListPendingCompletion<T>> obj)
            {
                localItems.AddRange(remoteItems);
            }
        }

        private IListPendingCompletion<T> SublistByKeyQuery(List<Guid> keyList)
        {
            var ret = source.ItemsByKeys(keyList);
            RegisterListForReconciliation(ret);
            return ret;
        }
    }
}