using System.Collections.Generic;
using System.Linq;

namespace Planner.Models.Tasks
{
    public static class PickTaskPriority
    {
        public static void PickPriority(this IList<PlannerTask> list, PlannerTask task, char currentPriority)
        {
            switch (task.Priority, task.Order)
            {
                case (' ', _):
                    task.Priority = currentPriority;
                    break;
                case (_, 0):
                    task.Order = list.ComputeNextOrder(task.Priority);
                    break;
                default:
                    list.ChangeTaskPriority(task, ' ', 0);
                    break;
            }
        }

        private static int ComputeNextOrder(this IList<PlannerTask> list, char priority) =>
            list.Where(i => i.Priority == priority).Max(i => i.Order) + 1;

        public static void SetImpliedOrders(this IList<PlannerTask> list)
        {
            if (list.AnyTaskLacksAPriority()) return;
            foreach (var task in list.SingleUnassignedTasksWithinTheirPriority())
            {
                list.PickPriority(task, 'A');
            }
        }

        private static IEnumerable<PlannerTask>
            SingleUnassignedTasksWithinTheirPriority(this IList<PlannerTask> list) =>
            list
                .Where(i => i.Order < 1)
                .GroupBy(i => i.Priority)
                .Where(i => i.Count() == 1)
                .Select(i => i.First());

        private static bool AnyTaskLacksAPriority(this IList<PlannerTask> list) => list.Any(i => i.Priority == ' ');

        public static bool CompletelyPrioritized(this IList<PlannerTask> list)
        {
            return (list.All(i => i.Prioritized));
        }
    }
}