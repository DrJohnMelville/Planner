using System.Linq;
using Planner.Models.Tasks;
using Xunit;

namespace Planner.Models.Test.Tasks
{
    public class TaskNameParserTest
    {
        private readonly TaskNameParser sut = new TaskNameParser();

        [Fact]
        public void TestName()
        {
            var list = sut.Parse("Foo");
            Assert.Single(list);
            
        }

    }
}