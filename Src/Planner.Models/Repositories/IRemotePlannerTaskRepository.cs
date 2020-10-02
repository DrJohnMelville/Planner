using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Melville.MVVM.Time;
using NodaTime;
using Planner.Models.Tasks;

namespace Planner.Models.Repositories
{
    public interface IRemoteDatum
    {
        Task<bool> WaitForOverridingEvent(IWallClock waiter, TimeSpan timeout);
    }

    public abstract class PlannerDataBase : IRemoteDatum
    {
        public Guid Key { get; set; }
        
        private volatile int updateCount = int.MinValue;
        int NewUpdateCount() => Interlocked.Increment(ref updateCount);
        bool UnchangedSince(int lastUpdateCount) => updateCount == lastUpdateCount;

        async Task<bool> IRemoteDatum.WaitForOverridingEvent(IWallClock waiter, TimeSpan timeout)
        {
            var invocationCount = NewUpdateCount();
            await waiter.Wait(timeout);
            return UnchangedSince(invocationCount);
        }
    }
    
    public interface IPlannerTaskRepository
    {
        Task AddTask(PlannerTask task);
        Task UpdateTask(PlannerTask task);
        Task DeleteTask(PlannerTask task);
        IAsyncEnumerable<PlannerTask> TasksForDate(LocalDate date);
    }
}