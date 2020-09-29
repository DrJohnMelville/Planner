using NodaTime;
using Planner.Models.Repositories;
using Planner.Models.Tasks;

namespace Planner.Repository
{
    public class TemporaryPTF : ILocalPlannerTaskRepository
    {
        public PlannerTask CreateTask(string name, LocalDate date)
        {
            return new PlannerTask() {Name = name};
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