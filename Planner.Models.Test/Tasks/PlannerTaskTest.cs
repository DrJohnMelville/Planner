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
            sut.Status = PlannerTaskStatus.Cancelled;
            Assert.Equal(PlannerTaskStatus.Cancelled, sut.Status);
               
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
            using var _ = INPCCounter.VerifyInpcFired(sut, i => i.Order, i => i.PriorityDisplay);
            sut.Order = order;
            Assert.Equal(display, sut.PriorityDisplay);
            
        }
    }
}