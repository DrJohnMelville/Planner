using System.Collections.Generic;

namespace Planner.Models.Tasks
{
    public static class ChargeTaskPriorityImpl
    {
        public static void ChangeTaskPriority(this IList<PlannerTask> list, PlannerTask task,
            char priority, int order)
        {
            CollapseAround(list, task.Priority, task.Order);
            MakeSpaceFor(list, priority, order);
            task.Priority = priority;
            task.Order = order;
        }

        private static void CollapseAround(IList<PlannerTask> list, in char priority, in int order)
        {
            if (IsUnassignedPriority(priority, order)) return;
            foreach (var plannerTask in list)
            {
                if (plannerTask.Priority == priority && plannerTask.Order > order)
                {
                    plannerTask.Order--;
                }
            }
        }

        private static void MakeSpaceFor(IList<PlannerTask> list, in char priority, in int order)
        {
            if (IsUnassignedPriority(priority, order)) return;
            foreach (var plannerTask in list)
            {
                if (plannerTask.Priority == priority && plannerTask.Order >= order)
                {
                    plannerTask.Order++;
                }
            }
        }

        private static bool IsUnassignedPriority(char priority, int order)
        {
            return priority == ' ' || order == 0;
        }
    }
}