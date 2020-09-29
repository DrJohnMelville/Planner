using NodaTime;
using Planner.Models.Tasks;

namespace Planner.Models.Repositories
{
    public interface ILocalPlannerTaskRepository
    {
        PlannerTask CreateTask(string name, LocalDate date);
        PlannerTaskList TasksForDate(LocalDate date);
    }
}