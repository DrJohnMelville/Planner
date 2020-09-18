using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Melville.MVVM.AdvancedLists;

namespace Planner.Models.Tasks
{
    public class PlannerTaskList: ThreadSafeBindableCollection<PlannerTask>
    {
        public void PickPriority(PlannerTask task, char currentPriority)
        {
            switch (task.Priority, task.Order)
            {
                case (' ', _):
                    task.Priority = currentPriority;
                    break;
                case (_, 0):
                    task.Order = ComputeNextOrder(task.Priority);
                    break;
                default:
                    task.Order = 0;
                    task.Priority = ' ';
                    break;
            }
        }

        private int ComputeNextOrder(char priority) => this.Where(i=>i.Priority == priority).Max(i => i.Order) + 1;


        public void SetImpliedOrders()
        {
            if (AnyTaskLacksAPriority()) return;
            foreach (var task in SingleUnassignedTasksWithinTheirPriority())
            {
                PickPriority(task, 'A');                              
            }
        }

        private IEnumerable<PlannerTask> SingleUnassignedTasksWithinTheirPriority() =>
            this
                .Where(i=>i.Order < 1)
                .GroupBy(i=>i.Priority)
                .Where(i=>i.Count() == 1)
                .Select(i=>i.First());

        private bool AnyTaskLacksAPriority() => this.Any(i => i.Priority == ' ');

        public bool CompletelyPrioritized()
        {
            return (this.All(i => i.Prioritized));
        }
    }
}