using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.Tasks;

namespace Planner.Models.Repositories
{
    public sealed class RemotePlannerTask : PlannerTask
    {
        private Guid key;
        public Guid Key => key;

        public RemotePlannerTask(Guid key)
        {
            this.key = key;
        }

        public RemotePlannerTask():this(Guid.Empty)
        {
        }

        private volatile int updateCount = int.MinValue;
        public int NewUpdateCount() => Interlocked.Increment(ref updateCount);
        public bool UnchangedSince(int lastUpdateCount) => updateCount == lastUpdateCount;
    }
    public interface IRemotePlannerTaskRepository
    {
        Task AddOrUpdateTask(RemotePlannerTask task);
        Task DeleteTask(RemotePlannerTask task);
        IAsyncEnumerable<RemotePlannerTask> TasksForDate(LocalDate date);
    }
}