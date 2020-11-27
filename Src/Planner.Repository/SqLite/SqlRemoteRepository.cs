using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Planner.Models.Repositories;

namespace Planner.Repository.SqLite
{
    public class SqlRemoteRepository<T>: IRemoteRepository<T> where T:PlannerItemBase
    {
        protected readonly Func<PlannerDataContext> contextFactory;
 
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

        public IAsyncEnumerable<T> ItemsFromKeys(IEnumerable<Guid> keys)
        {
            var capturedKeys = keys.ToList();
            return SimpleQuery(i => capturedKeys.Contains(i.Key));
        }

        protected IAsyncEnumerable<T> SimpleQuery(Expression<Func<T, bool>> predicate)
        {
            var ctx = contextFactory();
            return new DisposeWithAsyncEnumerable<T>(ctx.Set<T>()
                .AsNoTracking()
                .Where(predicate)
                .AsAsyncEnumerable(), ctx);
        }
    }
}