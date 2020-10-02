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
}