using System;
using System.Collections.Generic;
using Planner.Models.Tasks;

namespace Planner.WpfViewModels.TaskList
{
    public class PriorityKey
    {
        public char Priority { get; }
        public int Order { get; }
        public string Display => Order > 0? $"{Priority}{Order}":"Remove Priority";

        public PriorityKey(char priority, int order)
        {
            Priority = priority;
            Order = order;
        }
    }

    public static class PriortyKeyListFactory
    {
        public static IEnumerable<PriorityKey> CreatePriorityMenu(this IList<PlannerTask> tasks)
        {
            var orders = ComputeMaxPriorities(tasks);

            yield return new PriorityKey(' ', 0);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < orders[i]; j++)
                {
                    yield return new PriorityKey((char)('A' + i), j + 1);
                }
            }
        }

        private static int[] ComputeMaxPriorities(IList<PlannerTask> tasks)
        {
            int[] orders = new[] {1, 1, 1, 1};
            foreach (var task in tasks)
            {
                var index = task.Priority - 'A';
                if (IsConcretePriorityIndex(index))
                {
                    orders[index] = Math.Max(orders[index], task.Order + 1);
                }
            }
            return orders;
        }
        private static bool IsConcretePriorityIndex(int index) => index >= 0 && index < 4;
    }
}