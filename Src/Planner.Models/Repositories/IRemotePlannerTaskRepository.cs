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
        public Guid Key { get; set; }

        public RemotePlannerTask(Guid key)
        {
            Key = key;
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