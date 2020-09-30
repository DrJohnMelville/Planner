using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.Repositories;
using Planner.Repository.SqLite;
using Xunit;

namespace Planner.Repository.Test.SqLite
{
    public class SqlPlanerTaskRepositoryTest
    {
        private readonly TestDatabase data = new TestDatabase();
        private readonly SqlPlannerTaskRepository sut;
        private readonly LocalDate date1 = new LocalDate(1975,07,28);
        private readonly LocalDate date2 = new LocalDate(1974,08,18);

        public SqlPlanerTaskRepositoryTest()
        {
            sut = new SqlPlannerTaskRepository(data.NewContext);
        }

        [Fact]
        public async Task CreatingATaskAddsItToTheListForTheDay()
        {
            var pt = new RemotePlannerTask(Guid.Empty) {Name = "Foo", Date = date1};
            await sut.AddOrUpdateTask(pt);
            var list = await sut.TasksForDate(date1).ToListAsync();
            Assert.Single(list);
            Assert.Equal("Foo", list[0].Name);
        }
        [Fact]
        public async Task ModificationsGetSaved()
        {
            var pt = new RemotePlannerTask(Guid.Empty) {Name = "Foo", Date = date1};
            await sut.AddOrUpdateTask(pt);
            pt.Name = "Bar";
            await sut.AddOrUpdateTask(pt);
            var list = await sut.TasksForDate(date1).ToListAsync();
            Assert.Single(list);
            Assert.Equal("Bar", list[0].Name);
        }
        [Fact]
        public async Task TasksOnlyQueryFromTheIndicatedDate()
        {
            var pt = new RemotePlannerTask(Guid.Empty) {Name = "Foo", Date = date1};
            await sut.AddOrUpdateTask(pt);
            var list = await sut.TasksForDate(date2).ToListAsync();
            Assert.Empty(list);
        }
        [Fact]
        public async Task DeleteTask()
        {
            var pt = new RemotePlannerTask(Guid.Empty) {Name = "Foo", Date = date1};
            await sut.AddOrUpdateTask(pt);
            var list = await sut.TasksForDate(date1).ToListAsync();
            Assert.Single(list);
            await sut.DeleteTask(pt);
            Assert.Empty(await sut.TasksForDate(date1).ToListAsync());
        }
    }
    
    public static class AsyncEnumerableExtensions
    {
        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> items)
        {
            var results = new List<T>();
            await foreach (var item in items)
                results.Add(item);
            return results;
        }
    }
}