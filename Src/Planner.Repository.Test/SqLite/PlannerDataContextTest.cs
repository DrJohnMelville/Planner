using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Planner.Models.Tasks;
using Xunit;

namespace Planner.Repository.Test.SqLite
{
    public class PlannerDataContextTest
    {
        private readonly TestDatabase data = new TestDatabase();
        [Fact]
        public async Task RoundTripPlannerTask()
        {
            await using (var ctx = data.NewContext())
            {
                var pt = new PlannerTask(Guid.Empty)
                {
                    Name = "Foo",
                    Priority = 'C',
                    Order = 3,
                    Date = new LocalDate(1975, 07,28)
                };
                ctx.PlannerTasks.Add(pt);
                await ctx.SaveChangesAsync();
            }

            await using var ctx2 = data.NewContext();
            var newPt = await ctx2.PlannerTasks.FirstOrDefaultAsync();
            Assert.Equal("Foo", newPt.Name);
            Assert.Equal('C', newPt.Priority);
            Assert.Equal(3, newPt.Order);
            Assert.Equal(new LocalDate(1975,07,28), newPt.Date);
        }
    }
}