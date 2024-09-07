using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Melville.Hacks;
using NodaTime;
using Planner.Models.HtmlGeneration;

namespace Planner.Models.Repositories
{
    public class ClearCachesEventArgs: EventArgs{}
    
    public interface ICachedRepositorySource<T> : ILocalRepository<T> 
    {
    }

    public abstract class CachedRepositoryBase<TKey, T> : ILocalRepository<T> 
        where T : class where TKey: notnull
    {
        private readonly Dictionary<LocalDate, WeakReference<IListPendingCompletion<T>>> listCache =
            new();

        private readonly Dictionary<TKey, WeakReference<T>> itemCache = new();

        private readonly ILocalRepository<T> source;

        protected abstract TKey KeyFromItem(T item);
        protected abstract TKey KeyFromGuid(Guid guid);

        protected abstract IListPendingCompletion<T> QuerySource(
            ILocalRepository<T> source, IList<TKey> key);

        public CachedRepositoryBase(ICachedRepositorySource<T> source, 
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
            var ret = CheckCacheForItems(keys.Select(KeyFromGuid));
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
                if (itemCache.TryGetValue(KeyFromItem(list[i]), out var weakRef))
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
                    itemCache[KeyFromItem(list[i])] = new WeakReference<T>(list[i]);
                }
            }
        }

        private IListPendingCompletion<T> CheckCacheForItems(IEnumerable<TKey> keys)
        {
            var (remoteKeysToLoad, locallyFoundItems) = SortKeysIntoLocalAndRemote(keys);
            return AddKeysToList(remoteKeysToLoad, locallyFoundItems);
        }

        private (List<TKey> remoteKeysToLoad, ItemList<T> locallyFoundItems) SortKeysIntoLocalAndRemote(
            IEnumerable<TKey> keys)
        {
            var remoteKeysToLoad = new List<TKey>();
            var locallyFoundItems = new ItemList<T>();
            foreach (var key in keys)
            {
                SortSingleKey(key, locallyFoundItems, remoteKeysToLoad);
            }

            return (remoteKeysToLoad, locallyFoundItems);
        }

        private void SortSingleKey(TKey key, ItemList<T> locallyFoundItems, List<TKey> remoteKeysToLoad)
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

        private IListPendingCompletion<T> AddKeysToList(List<TKey> remoteKeysToLoad, ItemList<T> ret) =>
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

        private IListPendingCompletion<T> SublistByKeyQuery(List<TKey> keyList)
        {
            var ret = QuerySource(source, keyList);
            RegisterListForReconciliation(ret);
            return ret;
        }
        
    } 
    
    public class CachedRepository<T> :CachedRepositoryBase<Guid, T> where T : PlannerItemWithDate
    {
        public CachedRepository(ICachedRepositorySource<T> source, IEventBroadcast<ClearCachesEventArgs> clearCacheSignal) : base(source, clearCacheSignal)
        {
        }

        protected override Guid KeyFromItem(T item) => item.Key;

        protected override Guid KeyFromGuid(Guid guid) => guid;

        protected override IListPendingCompletion<T> QuerySource(
            ILocalRepository<T> source, IList<Guid> key) => source.ItemsByKeys(key);
    }
}