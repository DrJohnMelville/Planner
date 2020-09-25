using NodaTime;
using Planner.Models.Tasks;

namespace Planner.Models.Repositories
{
    public interface IPlannerTaskRepository
    {
        PlannerTask CreateTask(string title, LocalDate date);
        PlannerTaskList TasksForDate(LocalDate date);
    }
}