using System;
using System.Collections.Generic;

namespace Planner.Models.Tasks
{
    public record PriorityKey(char Priority, int Order)
    {
        public string Display => Order > 0? $"{Priority}{Order}":"Remove Priority";
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