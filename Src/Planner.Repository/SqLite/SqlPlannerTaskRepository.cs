using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Planner.Models.Repositories;
using Planner.Models.Tasks;

namespace Planner.Repository.SqLite
{
    
    public class SqlPlannerTaskRepository: IPlannerTaskRepository
    {
        private Func<PlannerDataContext> contextFactory;

        public SqlPlannerTaskRepository(Func<PlannerDataContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        private async Task HandleTask(PlannerTask task, Action<DbSet<PlannerTask>, PlannerTask> action)
        {
            VerifyProperKey(task);
            using var ctx = contextFactory();
            action(ctx.PlannerTasks, task);
            await ctx.SaveChangesAsync();
        }

        public Task AddTask(PlannerTask task) => HandleTask(task, (table, rt) => table.Add(rt)); 
        public Task UpdateTask(PlannerTask task) => HandleTask(task, (table, rt) => table.Update(rt)); 
        public Task DeleteTask(PlannerTask task) => HandleTask(task, (table, rt) =>
        {
            table.Attach(task);
            table.Remove(task);
        });

        [Conditional("DEBUG")]
        private static void VerifyProperKey(PlannerTask task)
        {
            if (task.Key == Guid.Empty) throw new InvalidOperationException("Invalid GUID");
        }

        public async IAsyncEnumerable<PlannerTask> TasksForDate(LocalDate date)
        {
            await using var ctx = contextFactory();
            // enumerate to make sure that the implicit dispose block is correct
            await foreach (var item in PlannerTaskByDateQuery(ctx, date))
            {
                yield return item;
            }
        }

        private static IAsyncEnumerable<PlannerTask> PlannerTaskByDateQuery(
            PlannerDataContext ctx, LocalDate date) =>
            ctx.PlannerTasks
                .AsNoTracking()
                .Where(i => i.Date == date)
                .AsAsyncEnumerable();
    }
}