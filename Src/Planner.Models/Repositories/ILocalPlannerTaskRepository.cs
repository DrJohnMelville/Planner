using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.Tasks;
using Planner.Models.Time;

namespace Planner.Models.Repositories
{
    public interface ILocalRepository<T> where T: PlannerItemWithDate
    {
         T CreateItem(LocalDate date, Action<T> initialize);
         IListPendingCompletion<T> ItemsForDate(LocalDate date);
         IListPendingCompletion<T> ItemsByKeys(IEnumerable<Guid> keys);
    }

    public static class PlannerTaskLocalRepoOperations
    {
        public static PlannerTask CreateTask(this ILocalRepository<PlannerTask> list, string name, LocalDate date) => 
            list.CreateItem(date, i=>i.Name = name);
        
        public static bool TryItemsForDate<T>(
            this ILocalRepository<T> repo, string dateStr, 
            [NotNullWhen(true)]out IListPendingCompletion<T>? output)
            where T: PlannerItemWithDate
        {
            if (TimeOperations.TryParseLocalDate(dateStr, out var localDate))
            {
                output = repo.ItemsForDate(localDate);
                return true;
            }
            output = null;
            return false;
        }

        public static async Task<T?> ItemOnDate<T>(
            this ILocalRepository<T> repo, string date, string key)
            where T: PlannerItemWithDate
        {
            if (!(Guid.TryParse(key, out var guidKey) &&
                  repo.TryItemsForDate(date, out var list))) return null;
            await list.CompleteList();
            return list.FirstOrDefault(i => i.Key == guidKey);
        }

        public static void DeferToDate(this ILocalRepository<PlannerTask> tasks, PlannerTask task,
            LocalDate newDate)
        {
            tasks.CreateTask(task.Name, newDate);
            task.Status = PlannerTaskStatus.Deferred;
            task.StatusDetail = newDate.ToString("D", null);
        }
    }
}