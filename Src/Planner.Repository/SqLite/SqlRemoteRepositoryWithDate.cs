using System;
using System.Collections.Generic;
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

        public IAsyncEnumerable<T> TasksForDate(LocalDate date, DateTimeZone timeZone)
        {
            return SimpleQuery(i => i.Date == date);
        }

    }
}