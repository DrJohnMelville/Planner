using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Planner.Models.Repositories;

namespace Planner.Repository.SqLite
{
    public class SqlItemByKeyRepository<T> : IItemByKeyRepository<T> where T : PlannerItemBase
    {
        private Func<PlannerDataContext> dbCreator;

        public SqlItemByKeyRepository(Func<PlannerDataContext> dbCreator)
        {
            this.dbCreator = dbCreator;
        }

        public async Task<T?> ItemByKey(Guid key)
        {
            await using var db = dbCreator();
            return await db.Set<T>().FirstOrDefaultAsync(i => i.Key == key);
        }
    }
}