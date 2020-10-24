
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;

namespace Planner.Models.Repositories
{
    public interface IRemoteRepository<T>
    {
        Task Add(T task);
        Task Update(T task);
        Task Delete(T task);
    }
    public interface IDatedRemoteRepository<T>: IRemoteRepository<T> where T: PlannerItemWithDate
    {
        IAsyncEnumerable<T> TasksForDate(LocalDate date);
    }

    public interface IItemByKeyRepository<T> where T : PlannerItemBase
    {
        Task<T?> ItemByKey(Guid key);
    }
}