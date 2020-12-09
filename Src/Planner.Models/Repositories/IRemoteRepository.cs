
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;
using NodaTime.TimeZones;

namespace Planner.Models.Repositories
{
    public interface IRemoteRepository<T>
    {
        Task Add(T task);
        Task Update(T task);
        Task Delete(T task);
        IAsyncEnumerable<T> ItemsFromKeys(IEnumerable<Guid> keys);
        
    }
    public interface IDatedRemoteRepository<T>: IRemoteRepository<T> 
    {
        IAsyncEnumerable<T> TasksForDate(LocalDate date, DateTimeZone timeZone);
    }
}