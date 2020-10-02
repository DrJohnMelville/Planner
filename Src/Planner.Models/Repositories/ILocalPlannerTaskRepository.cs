using System.Collections.Generic;
using NodaTime;
using Planner.Models.Tasks;

namespace Planner.Models.Repositories
{
    // public interface ILocalRepository<T> where T: PlannerItemWithDate
    // {
    //     T CreateTask(string name, LocalDate date);
    //     PlannerTaskList TasksForDate(LocalDate date);
    // }
    public interface ILocalPlannerTaskRepository
    {
        PlannerTask CreateTask(string name, LocalDate date);
        IList<PlannerTask> TasksForDate(LocalDate date);
    }
}