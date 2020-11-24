using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Planner.Models.Repositories;

namespace Planner.Repository.SqLite
{
    public class SqlRemoteRepositoryWithDate<T>: SqlRemoteRepository<T>, IDatedRemoteRepository<T>
        where T: PlannerItemWithDate
    {
        public SqlRemoteRepositoryWithDate(Func<PlannerDataContext> contextFactory) : base(contextFactory)
        {
        }

        public IAsyncEnumerable<T> TasksForDate(LocalDate date)
        {
            var ctx = contextFactory();
            return new DisposeWithAsyncEnumerable<T>(PlannerTaskByDateQuery(ctx, date), ctx);
        }

        private static IAsyncEnumerable<T> PlannerTaskByDateQuery(
            PlannerDataContext ctx, LocalDate date) =>
            ctx.Set<T>()
                .AsNoTracking()
                .Where(i => i.Date == date)
                .AsAsyncEnumerable();
    }
}