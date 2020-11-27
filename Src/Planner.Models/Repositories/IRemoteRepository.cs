
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NodaTime;

namespace Planner.Models.Repositories
{
    public interface IRemoteRepository<T>
    {
        Task Add(T task);
        Task Update(T task);
        Task Delete(T task);
        IAsyncEnumerable<T> ItemsFromKeys(IEnumerable<Guid> keys);
        
    }
    public interface IDatedRemoteRepository<T>: IRemoteRepository<T> where T: PlannerItemWithDate
    {
        IAsyncEnumerable<T> TasksForDate(LocalDate date);
    }

    public static class RemoteRepositoryOperations
    {
        public static ValueTask<T?> ItemByKey<T>(this IRemoteRepository<T> repo, Guid key) =>
             repo.ItemsFromKeys(new[] {key}).FirstOrDefaultAsync();
    }
}