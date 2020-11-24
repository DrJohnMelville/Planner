using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.Notes;
using Planner.Repository.SqLite;
using Xunit;

namespace Planner.Repository.Test.SqLite
{
    public class SqlNoteSearcherTest
    {
        private readonly TestDatabase data = new TestDatabase();
        private readonly SqlNoteSearcher sut;

        public SqlNoteSearcherTest()
        {
            FillWithSampleNotes(data.NewContext());
            sut = new SqlNoteSearcher(data.NewContext);
        }

        private void FillWithSampleNotes(PlannerDataContext ctx)
        {
            CreateDat(ctx, 20, "TFoo20", "Foo20");
            CreateDat(ctx, 21, "TFoo21", "BFoo21");
            CreateDat(ctx, 22, "TFoo22", "BFoo22");
            CreateDat(ctx, 23, "TFoo23", "BFoo23");
            CreateDat(ctx, 24, "TFoo24", "BFoo24");
            CreateDat(ctx, 25, "TFoo25", "BFoo25");
            CreateDat(ctx, 26, "TFoo26", "BFoo26");
            CreateDat(ctx, 27, "TFoo27", "BFoo27");
            CreateDat(ctx, 28, "TFoo28", "BFoo28");
            CreateDat(ctx, 29, "Foo29", "BFoo29");
            ctx.SaveChanges();
        }

        private static void CreateDat(PlannerDataContext ctx, int day, string title, string text)
        {
            ctx.Notes.Add(new Note()
            {
                Key = Guid.NewGuid(), Title = title, Text = text,
                Date = new LocalDate(1975, 7, day),
                TimeCreated = SystemClock.Instance.GetCurrentInstant()
            });
        }

        [Theory]
        [InlineData("Foo", 19, 31, 10)]
        [InlineData("Foo", 21, 23, 3)]
        [InlineData("Foo", 23, 23, 1)]
        [InlineData("TFoo", 19, 31, 9)]
        [InlineData("BFoo", 19, 31, 9)]
        [InlineData("TFoo23", 19, 31, 1)]
        public async Task TestName(string query, int minday, int maxday, int count)
        {
            Assert.Equal(count, await sut.SearchFor(query, new LocalDate(1975,7,minday),
                new LocalDate(1975,7,maxday)).CountAsync());
            
        }

    }
}