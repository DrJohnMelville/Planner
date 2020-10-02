using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Planner.Models.Repositories;

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

        public Task Add(T task) => HandleTask(task, (table, rt) => table.Add(rt)); 
        public Task Update(T task) => HandleTask(task, (table, rt) => table.Update(rt)); 
        public Task Delete(T task) => HandleTask(task, (table, rt) =>
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
}