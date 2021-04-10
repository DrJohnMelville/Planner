using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Melville.Hacks;
using Melville.Lists;
using Melville.SystemInterface.Time;
using NodaTime;
using Planner.Models.Time;

namespace Planner.Models.Repositories
{
    public  class LocalToRemoteRepositoryBridge<T>:ICachedRepositorySource<T> 
       where T:INotifyPropertyChanged, new()
    {
        private readonly IDatedRemoteRepository<T> remote;
        private readonly IWallClock waiter;
        private readonly IUsersClock usersClock;

        public LocalToRemoteRepositoryBridge(IDatedRemoteRepository<T> remote, 
            IWallClock waiter, IUsersClock usersClock)
        {
            this.remote = remote;
            this.waiter = waiter;
            this.usersClock = usersClock;
        }

        public T CreateItem(LocalDate date, Action<T> initialize)
        {
            var ret = ConstructNewItem(date, initialize);
            remote.Add(ret);
            RegisterPropertyChangeNotifications(ret);
            return ret;
        }

        private static T ConstructNewItem(LocalDate date, Action<T> initialize)
        {
            var ret = new T();
            TryPopulatePlannerItemBase(date, ret);
            initialize(ret);
            return ret;
        }

        private static void TryPopulatePlannerItemBase(LocalDate date, T ret)
        {
            if (ret is PlannerItemWithDate pib)
            {
                pib.Date = date;
                pib.Key = Guid.NewGuid();
            }
        }

        private void RegisterPropertyChangeNotifications(T plannerTask) => 
            plannerTask.PropertyChanged += DelayedUpdateEvent;

        private void DelayedUpdateEvent(object? sender, EventArgs _)
        {
            if (sender is T rpt) DelayedUpdate(rpt); 
        }
        private async void DelayedUpdate(T plannerTask)
        {
            if (await ((IRemoteDatum) plannerTask).WaitForOverridingEvent(waiter, TimeSpan.FromSeconds(2)))
                remote.Update(plannerTask).FireAndForget();
        }

        public IListPendingCompletion<T> ItemsForDate(LocalDate date) => 
            LoadItems(remote.TasksForDate(date, usersClock.CurrentUiTimeZone()));
        public IListPendingCompletion<T> ItemsByKeys(IEnumerable<Guid> keys) => 
            LoadItems(remote.ItemsFromKeys(keys));


        private void RegisterItemRemovalNotification(ThreadSafeBindableCollection<T> ret) => 
            ret.CollectionChanged += CollectionChanged;

        private void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
                RemoveItems(e.OldItems?.OfType<T>()??Array.Empty<T>());
        }

        private void RemoveItems(IEnumerable<T> oldTasks)
        {
            foreach (var task in oldTasks)
            {
                remote.Delete(task);
            }
        }

        private IListPendingCompletion<T> LoadItems(IAsyncEnumerable<T> tasksForDate)
        {
            var ret = new ItemList<T>();
            RegisterItemRemovalNotification(ret);
            ret.SetCompletionTask(FillResultList(tasksForDate, ret));
            return ret;
        }
        private async Task FillResultList(IAsyncEnumerable<T> itemsToAdd, IList<T> ret)
        {
            await foreach (var task in itemsToAdd)
            {
                RegisterPropertyChangeNotifications(task);
                ret.Add(task);
            }
        }
    }
}