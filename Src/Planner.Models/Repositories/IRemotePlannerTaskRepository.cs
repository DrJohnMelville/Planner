using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.Tasks;

namespace Planner.Models.Repositories
{
    public interface IRemoteDatum
    {
        int NewUpdateCount();
        bool UnchangedSince(int lastUpdateCount);
    }

    public abstract class PlannerDataBase : IRemoteDatum
    {
        public Guid Key { get; set; }
        private volatile int updateCount = int.MinValue;
        int IRemoteDatum.NewUpdateCount() => Interlocked.Increment(ref updateCount);
        bool IRemoteDatum.UnchangedSince(int lastUpdateCount) => updateCount == lastUpdateCount;
    }
    
    public interface IPlannerTaskRepository
    {
        Task AddTask(PlannerTask task);
        Task UpdateTask(PlannerTask task);
        Task DeleteTask(PlannerTask task);
        IAsyncEnumerable<PlannerTask> TasksForDate(LocalDate date);
    }
}