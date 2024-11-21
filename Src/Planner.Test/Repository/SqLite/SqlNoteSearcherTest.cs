using NodaTime;
using Planner.Models.Notes;
using Planner.Repository.SqLite;

namespace TUnit.Repository.SqLite;

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
        CreateData(ctx, 20, "TFoo20", "Foo20");
        CreateData(ctx, 21, "TFoo21", "BFoo21");
        CreateData(ctx, 22, "TFoo22", "BFoo22");
        CreateData(ctx, 23, "TFoo23", "BFoo23");
        CreateData(ctx, 24, "TFoo24", "BFoo24");
        CreateData(ctx, 25, "TFoo25", "BFoo25");
        CreateData(ctx, 26, "TFoo26", "BFoo26");
        CreateData(ctx, 27, "TFoo27", "BFoo27");
        CreateData(ctx, 28, "TFoo28", "BFoo28");
        CreateData(ctx, 29, "Foo29", "BFoo29");
        ctx.SaveChanges();
    }

    private static void CreateData(PlannerDataContext ctx, int day, string title, string text)
    {
        ctx.Notes.Add(new Note()
        {
            Key = Guid.NewGuid(), Title = title, Text = text,
            Date = new LocalDate(1975, 7, day),
            TimeCreated = SystemClock.Instance.GetCurrentInstant()
        });
    }

    [Test]
    [Arguments("Foo", 19, 31, 10)]
    [Arguments("Foo", 21, 23, 3)]
    [Arguments("Foo", 23, 23, 1)]
    [Arguments("TFoo", 19, 31, 9)]
    [Arguments("BFoo", 19, 31, 9)]
    [Arguments("TFoo23", 19, 31, 1)]
    public async Task TestName(string query, int minday, int maxday, int count)
    {
        Assert.Equal(count, await sut.SearchFor(query, new LocalDate(1975,7,minday),
            new LocalDate(1975,7,maxday)).CountAsync());
            
    }

}