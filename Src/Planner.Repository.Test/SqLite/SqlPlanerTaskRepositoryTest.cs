using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.Tasks;
using Planner.Repository.SqLite;
using Xunit;

namespace Planner.Repository.Test.SqLite
{
    public class SqlPlanerTaskRepositoryTest
    {
        private readonly TestDatabase data = new TestDatabase();
        private readonly SqlRemoteRepositoryWithDate<PlannerTask> sut;
        private readonly LocalDate date1 = new LocalDate(1975,07,28);
        private readonly LocalDate date2 = new LocalDate(1974,08,18);

        public SqlPlanerTaskRepositoryTest()
        {
            sut = new SqlRemoteRepositoryWithDate<PlannerTask>(data.NewContext);
        }

        [Fact]
        public async Task CreatingATaskAddsItToTheListForTheDay()
        {
            var pt = new PlannerTask(Guid.NewGuid()) {Name = "Foo", Date = date1};
            await sut.Add(pt);
            var list = await sut.TasksForDate(date1).ToListAsync();
            Assert.Single(list);
            Assert.Equal("Foo", list[0].Name);
        }
        [Fact]
        public async Task ModificationsGetSaved()
        {
            var pt = new PlannerTask(Guid.NewGuid()) {Name = "Foo", Date = date1};
            await sut.Add(pt);
            pt.Name = "Bar";
            await sut.Update(pt);
            var list = await sut.TasksForDate(date1).ToListAsync();
            Assert.Single(list);
            Assert.Equal("Bar", list[0].Name);
        }
        [Fact]
        public async Task TasksOnlyQueryFromTheIndicatedDate()
        {
            var pt = new PlannerTask(Guid.NewGuid()) {Name = "Foo", Date = date1};
            await sut.Add(pt);
            var list = await sut.TasksForDate(date2).ToListAsync();
            Assert.Empty(list);
        }
        [Fact]
        public async Task DeleteTask()
        {
            var pt = new PlannerTask(Guid.NewGuid()) {Name = "Foo", Date = date1};
            await sut.Add(pt);
            var list = await sut.TasksForDate(date1).ToListAsync();
            Assert.Single(list);
            await sut.Delete(pt);
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