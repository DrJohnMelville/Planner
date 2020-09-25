using NodaTime;
using Planner.Models.Repositories;
using Planner.Models.Tasks;

namespace Planner.Repository
{
    public class TemporaryPTF : IPlannerTaskRepository
    {
        public PlannerTask CreateTask(string title, LocalDate date)
        {
            return new PlannerTask() {Name = title};
        }

        public PlannerTaskList TasksForDate(LocalDate date)
        {
            var src = new PlannerTaskList();
            src.Add(new PlannerTask() {Name = "Task1"});
            src.Add(new PlannerTask() {Name = "Task2"});
            src.Add(new PlannerTask() {Name = "Task3"});
            src.Add(new PlannerTask() {Name = "Task4"});
            return src;
        }
    }
}