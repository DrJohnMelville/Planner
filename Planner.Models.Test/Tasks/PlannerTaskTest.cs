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
            sut.Name = "Foo";
            Assert.Equal("Foo", sut.Name);
        }

    }
}