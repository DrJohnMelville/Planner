using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.Tasks;

namespace Planner.Models.Repositories
{
    public interface ILocalRepository<T> where T: PlannerItemWithDate
    {
         T CreateItem(LocalDate date, Action<T> initialize);
         IList<T> ItemsForDate(LocalDate date);
         Task<IList<T>> CompletedItemsForDate(LocalDate date);
    }

    public static class PlannerTaskLocalRepoOperations
    {
        public static PlannerTask CreateTask(this ILocalRepository<PlannerTask> list, string name, LocalDate date) => 
            list.CreateItem(date, i=>i.Name = name);
    }
}