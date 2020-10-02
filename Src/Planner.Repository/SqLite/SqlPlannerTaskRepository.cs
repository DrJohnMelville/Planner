using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Planner.Models.Repositories;

namespace Planner.Repository.SqLite
{
    public class SqlPlannerTaskRepository: IRemotePlannerTaskRepository
    {
        private Func<PlannerDataContext> contextFactory;

        public SqlPlannerTaskRepository(Func<PlannerDataContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task AddOrUpdateTask(RemotePlannerTask task)
        {
            if (task.Key == Guid.Empty) throw new InvalidOperationException("Invalid GUID");
            using var ctx = contextFactory();
            if (await ctx.PlannerTasks.Where(i => i.Key == task.Key).AnyAsync())
            {
                ctx.PlannerTasks.Update(task);
            }
            else
            {
                ctx.PlannerTasks.Add(task);
            }// This is hacky -- I know when the adds are and I need to honor them
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteTask(RemotePlannerTask task)
        {
            await using var ctx = contextFactory();
            ctx.PlannerTasks.Attach(task);
            ctx.PlannerTasks.Remove(task);
            await ctx.SaveChangesAsync();
        }

        public async IAsyncEnumerable<RemotePlannerTask> TasksForDate(LocalDate date)
        {
            await using var ctx = contextFactory();
            // enumerate to make sure that the implicit dispose block is correct
            await foreach (var item in PlannerTaskByDateQuery(ctx, date))
            {
                yield return item;
            }
        }

        private static IAsyncEnumerable<RemotePlannerTask> PlannerTaskByDateQuery(
            PlannerDataContext ctx, LocalDate date) =>
            ctx.PlannerTasks
                .AsNoTracking()
                .Where(i => i.Date == date)
                .AsAsyncEnumerable();
    }
}