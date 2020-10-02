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
    public class SqlRemoteRepository<T>: IRemoteRepository<T> where T:class
    {
        protected Func<PlannerDataContext> contextFactory;
 
        public SqlRemoteRepository(Func<PlannerDataContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        private async Task HandleTask(T task, Action<DbSet<T>, T> action)
        {
            VerifyProperKey(task);
            using var ctx = contextFactory();
            action(ctx.Set<T>(), task);
            await ctx.SaveChangesAsync();
        }

        public Task AddTask(T task) => HandleTask(task, (table, rt) => table.Add(rt)); 
        public Task UpdateTask(T task) => HandleTask(task, (table, rt) => table.Update(rt)); 
        public Task DeleteTask(T task) => HandleTask(task, (table, rt) =>
        {
            table.Attach(task);
            table.Remove(task);
        });

        [Conditional("DEBUG")]
        private static void VerifyProperKey(T task)
        {
            if (task is PlannerItemBase pib && pib.Key == Guid.Empty) 
                throw new InvalidOperationException("Invalid GUID");
        }

        }
        
        public class SqlRemoteRepositoryWithDate<T>: SqlRemoteRepository<T>, IDatedRemoteRepository<T>
            where T: PlannerItemWithDate
        {
            public SqlRemoteRepositoryWithDate(Func<PlannerDataContext> contextFactory) : base(contextFactory)
            {
            }

            public async IAsyncEnumerable<T> TasksForDate(LocalDate date)
            {
                await using var ctx = contextFactory();
                // enumerate to make sure that the implicit dispose block is correct
                await foreach (var item in PlannerTaskByDateQuery(ctx, date))
                {
                    yield return item;
                }
            }

        private static IAsyncEnumerable<T> PlannerTaskByDateQuery(
            PlannerDataContext ctx, LocalDate date) =>
            ctx.Set<T>()
                .AsNoTracking()
                .Where(i => i.Date == date)
                .AsAsyncEnumerable();
    }
    
    public class SqlPlannerTaskRepository: SqlRemoteRepositoryWithDate<PlannerTask>, IPlannerTaskRemoteRepository
    {
        public SqlPlannerTaskRepository(Func<PlannerDataContext> contextFactory) : base(contextFactory)
        {
        }
    }
}