﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Melville.MVVM.AdvancedLists;
using Melville.MVVM.CSharpHacks;
using Melville.MVVM.Time;
using NodaTime;
using Planner.Models.Tasks;

namespace Planner.Models.Repositories
{
    public class LocalToRemoteRepositoryBridge<T>:ICachedRepositorySource<T> where T:PlannerItemWithDate, new()
    {
        private readonly IDatedRemoteRepository<T> remote;
        private readonly IWallClock waiter;

        public LocalToRemoteRepositoryBridge(IDatedRemoteRepository<T> remote, 
            IWallClock waiter)
        {
            this.remote = remote;
            this.waiter = waiter;
        }

        public T CreateItem(LocalDate date, Action<T> initialize)
        {
            var ret = new T(){Date = date, Key = Guid.NewGuid()};
            initialize(ret);
            remote.Add(ret);
            RegisterPropertyChangeNotifications(ret);
            return ret;
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

        public IListPendingCompletion<T> ItemsForDate(LocalDate date)
        {
            var ret = new ItemList<T>();
            RegisterItemRemovalNotification(ret);
            ret.SetCompletionTask(LoadItemsForDate(date, ret));
            return ret;
        }
        private void RegisterItemRemovalNotification(ThreadSafeBindableCollection<T> ret) => 
            ret.CollectionChanged += CollectionChanged;

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
                RemoveItems(e.OldItems.OfType<T>());
        }

        private void RemoveItems(IEnumerable<T> oldTasks)
        {
            foreach (var task in oldTasks)
            {
                remote.Delete(task);
            }
        }

        private async Task LoadItemsForDate(LocalDate date, IList<T> ret)
        {
            await foreach (var task in remote.TasksForDate(date))
            {
                RegisterPropertyChangeNotifications(task);
                ret.Add(task);
            }
        }
    }
}