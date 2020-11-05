using System.Collections.Generic;
using System.Linq;
using Planner.Models.Tasks;
using Planner.WpfViewModels.TaskList;
using Xunit;

namespace Planner.Wpf.Test.TaskList
{
    public class PriorityKeyListFactoryTest
    {
        private List<PlannerTask> tasks = new List<PlannerTask>();

        private static void AssertResult(List<PriorityKey> ret, params string[] expected)
        {
            Assert.Equal(expected, ret.Select(i => i.Display));
        }

        [Fact]
        public void EmpptyListProduces5Items()
        {
            var ret = tasks.CreatePriorityMenu().ToList();
            AssertResult(ret, "Remove Priority", "A1", "B1", "C1", "D1");
        }
        [Fact]
        public void UnPrioritizedCreates5Items()
        {
            tasks.Add(new PlannerTask());
            var ret = tasks.CreatePriorityMenu().ToList();
            AssertResult(ret, "Remove Priority", "A1", "B1", "C1", "D1");
        }
        [Fact]
        public void Priority1Makes5Items()
        {
            tasks.Add(new PlannerTask(){Priority='A', Order = 1});
            var ret = tasks.CreatePriorityMenu().ToList();
            AssertResult(ret, "Remove Priority", "A1", "A2", "B1", "C1", "D1");
        }        
        [Fact]
        public void ComplexCreate()
        {
            tasks.Add(new PlannerTask(){Priority='A', Order = 1});
            tasks.Add(new PlannerTask(){Priority='C', Order = 2});
            var ret = tasks.CreatePriorityMenu().ToList();
            AssertResult(ret, "Remove Priority", "A1", "A2", "B1", "C1", "C2", "C3", "D1");
        }
    }
}