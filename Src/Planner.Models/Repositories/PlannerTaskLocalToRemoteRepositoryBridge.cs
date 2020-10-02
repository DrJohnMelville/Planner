using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Melville.MVVM.CSharpHacks;
using Melville.MVVM.Time;
using NodaTime;
using Planner.Models.Tasks;

namespace Planner.Models.Repositories
{
    public class PlannerTaskLocalToRemoteRepositoryBridge:ILocalPlannerTaskRepository
    {
        private readonly IRemotePlannerTaskRepository remote;
        private readonly IWallClock waiter;

        public PlannerTaskLocalToRemoteRepositoryBridge(IRemotePlannerTaskRepository remote, 
            IWallClock waiter)
        {
            this.remote = remote;
            this.waiter = waiter;
        }

        public PlannerTask CreateTask(string name, LocalDate date)
        {
            var ret = new RemotePlannerTask(Guid.NewGuid()){Name = name, Date = date};
            remote.AddTask(ret);
            RegisterPropertyChangeNotifications(ret);
            return ret;
        }

        private void RegisterPropertyChangeNotifications(RemotePlannerTask plannerTask) => 
            plannerTask.PropertyChanged += DelayedUpdateEvent;

        private void DelayedUpdateEvent(object? sender, EventArgs _)
        {
            if (sender is RemotePlannerTask rpt) DelayedUpdate(rpt); 
        }
        private async void DelayedUpdate(RemotePlannerTask plannerTask)
        {
            var updateCounter = plannerTask.NewUpdateCount();
            await waiter.Wait(TimeSpan.FromSeconds(2));
            if (!plannerTask.UnchangedSince(updateCounter)) return;
            remote.UpdateTask(plannerTask).FireAndForget();
        }

        public PlannerTaskList TasksForDate(LocalDate date)
        {
            var ret = new PlannerTaskList();
            RegisterItemRemovalNotification(ret);
            LoadTasksForDate(date, ret);
            return ret;
        }

        private void RegisterItemRemovalNotification(PlannerTaskList ret) => 
            ret.CollectionChanged += CollectionChanged;

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
                RemoveItems(e.OldItems.OfType<RemotePlannerTask>());
        }

        private void RemoveItems(IEnumerable<RemotePlannerTask> oldTasks)
        {
            foreach (var task in oldTasks)
            {
                remote.DeleteTask(task);
            }
        }

        private async void LoadTasksForDate(LocalDate date, PlannerTaskList ret)
        {
            await foreach (var task in remote.TasksForDate(date))
            {
                RegisterPropertyChangeNotifications(task);
                ret.Add(task);
            }
        }
    }
}