using System;
using System.Threading;
using System.Threading.Tasks;
using Melville.INPC;
using Melville.MVVM.Time;
using NodaTime;

namespace Planner.Models.Repositories
{
    public interface IRemoteDatum
    {
        Task<bool> WaitForOverridingEvent(IWallClock waiter, TimeSpan timeout);
    }

    public abstract class PlannerItemBase : IRemoteDatum
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
    
    public abstract partial class PlannerItemWithDate : PlannerItemBase
    {
        [AutoNotify] private LocalDate date;
    }

}