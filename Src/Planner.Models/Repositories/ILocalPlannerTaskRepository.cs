using System.Collections.Generic;
using NodaTime;
using Planner.Models.Tasks;

namespace Planner.Models.Repositories
{
    public interface ILocalRepository<T> where T: PlannerItemWithDate
    {
         T CreateTask(LocalDate date);
         IList<T> TasksForDate(LocalDate date);
    }

    public static class PlannerTaskLocalRepoOperations
    {
        public static PlannerTask CreateTask(this ILocalRepository<PlannerTask> list, string name, LocalDate date)
        {
            var ret = list.CreateTask(date);
            ret.Name = name;
            return ret;
        }
    }
}