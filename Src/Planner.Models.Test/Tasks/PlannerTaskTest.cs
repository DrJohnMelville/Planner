using Melville.TestHelpers.InpcTesting;
using Planner.Models.Tasks;
using Xunit;

namespace Planner.Models.Test.Tasks
{
    public class PlannerTaskTest
    {
        private readonly PlannerTask sut = new PlannerTask();
        [Fact]
        public void TaskHasName()
        {
            Assert.Equal("", sut.Name);
            sut.Name = "Foo";
            Assert.Equal("Foo", sut.Name);
        }

        [Fact]
        public void TaskHasPriority()
        {
            Assert.Equal(' ', sut.Priority);
            sut.Priority = 'A';
            Assert.Equal('A', sut.Priority);
        }

        [Fact]
        public void TaskHasOrder()
        {
            Assert.Equal(0, sut.Order);
            sut.Order = 12;
            Assert.Equal(12, sut.Order);
            
        }

        [Fact]
        public void TaskHasStatus()
        {
            Assert.Equal(PlannerTaskStatus.Incomplete, sut.Status);
            sut.Status = PlannerTaskStatus.Canceled;
            Assert.Equal(PlannerTaskStatus.Canceled, sut.Status);
               
        }

        [Fact]
        public void TaskHasStatusDetail()
        {
            Assert.Equal("", sut.StatusDetail);
            sut.StatusDetail = "7/28/1975";
            Assert.Equal("7/28/1975", sut.StatusDetail);
            
        }

        [Theory]
        [InlineData(' ', 0, " ")]
        [InlineData('A', 7, "A7")]
        [InlineData('A', 10, "A10")]
        [InlineData('B', 7, "B7")]
        [InlineData('B', 0, "B")]
        public void PriorityOrder(char priority, int order, string display)
        {
            sut.Priority = priority;
            using var _ = INPCCounter.VerifyInpcFired(sut, i => i.Order, i=>i.Prioritized, i => i.PriorityDisplay);
            sut.Order = order;
            Assert.Equal(display, sut.PriorityDisplay);
        }

        [Theory]
        [InlineData('A', 1, "Foo", 'A', 1, "Foo", 0)]
        [InlineData('A', 1, "Foo", ' ', 1, "Foo", 33)]
        [InlineData('C', 1, "Foo", 'A', 1, "Foo", 2)]
        [InlineData('A', 1, "Foo", 'C', 1, "Foo", -2)]
        [InlineData('A', 2, "Foo", 'A', 1, "Foo", 1)]
        [InlineData('A', 1, "Foo", 'A', 2, "Foo", -1)]
        [InlineData('A', 1, "Foo", 'A', 1, "Aoo", 1)]
        [InlineData('A', 1, "Foo", 'A', 1, "Zoo", -1)]
        [InlineData('A', 1, "Foo", 'A', 2, "Bar", -1)]
        public void SortLevel(char p1, int o1, string name1, char p2, int o2, string name2, int comp)
        {
            sut.Priority = p1;
            sut.Order = o1;
            sut.Name = name1;
            var other = new PlannerTask();
            other.Priority = p2;
            other.Order = o2;
            other.Name = name2;

            Assert.Equal(comp, sut.CompareTo(other));
        }

        [Theory]
        [InlineData(PlannerTaskStatus.Incomplete, PlannerTaskStatus.Done)]
        [InlineData(PlannerTaskStatus.Done, PlannerTaskStatus.Pending)]
        [InlineData(PlannerTaskStatus.Pending, PlannerTaskStatus.Canceled)]
        [InlineData(PlannerTaskStatus.Canceled, PlannerTaskStatus.Incomplete)]
        [InlineData(PlannerTaskStatus.Delegated, PlannerTaskStatus.Done)]
        [InlineData(PlannerTaskStatus.Deferred, PlannerTaskStatus.Deferred)]
        public void ToggleStatus(PlannerTaskStatus prior, PlannerTaskStatus next)
        {
            sut.Status = prior;
            sut.ToggleStatus();
            Assert.Equal(next, sut.Status);
            
        }

    }
}