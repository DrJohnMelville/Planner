using System.Collections.Generic;
using System.Linq;
using Planner.Models.Tasks;
using Xunit;

namespace Planner.Models.Test.Tasks
{
    public class ChangeTaskPriorityTest
    {
        private readonly List<PlannerTask> list = new();
        private readonly PlannerTask item = new();

        public ChangeTaskPriorityTest()
        {
            list.Add(item);
        }

        private void AssertCompleteOrder(string expected)
        {
            Assert.Equal(expected, string.Join("|", list.Select(i => i.PriorityDisplay)));
        }

        [Fact]
        public void InsertEmpty()
        {
            list.ChangeTaskPriority(item, 'A',3);
            AssertCompleteOrder("A3");
        }

        [Fact]
        public void DisplaceSingle()
        {
            list.Add(new PlannerTask(){Priority='A', Order = 1});
            list.ChangeTaskPriority(item,'A', 1);
            AssertCompleteOrder("A1|A2");
        }
        [Fact]
        public void DoNotAdjustDifferentPriority()
        {
            list.Add(new PlannerTask(){Priority='B', Order = 1});
            list.ChangeTaskPriority(item,'A', 1);
            AssertCompleteOrder("A1|B1");
        }

        [Fact]
        public void DoNotAdjustNullPriority()
        {
            list.Add(new PlannerTask());
            list.ChangeTaskPriority(item,'A', 1);
            AssertCompleteOrder("A1| ");
            list.ChangeTaskPriority(item, ' ', 0);
            AssertCompleteOrder(" | ");
        }

        [Fact]
        public void MoveWithinOrder()
        {
            list.ChangeTaskPriority(item, 'A',2);
            list.Add(new PlannerTask(){Priority='A', Order = 1});
            list.Add(new PlannerTask(){Priority='A', Order = 3});
            list.Add(new PlannerTask(){Priority='A', Order = 4});
            list.ChangeTaskPriority(item, 'A', 3);
            AssertCompleteOrder("A3|A1|A2|A4");
        }
        [Fact]
        public void IgnoreConfounders()
        {
            list.ChangeTaskPriority(item, 'A',2);
            list.Add(new PlannerTask(){Priority='A', Order = 1});
            list.Add(new PlannerTask(){Priority='A', Order = 3});
            list.Add(new PlannerTask(){Priority='A', Order = 4});
            list.Add(new PlannerTask(){Priority='B', Order = 1});
            list.Add(new PlannerTask(){Priority='B', Order = 2});
            list.Add(new PlannerTask(){Priority='B', Order = 3});
            list.Add(new PlannerTask(){Priority='B', Order = 4});
            list.Add(new PlannerTask());
            list.ChangeTaskPriority(item, 'A', 3);
            AssertCompleteOrder("A3|A1|A2|A4|B1|B2|B3|B4| ");
        }

    }
}