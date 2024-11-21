using NodaTime;
using Planner.Models.Notes;
using Planner.Web.Controllers;

namespace TUnit.Web;

public class SearchNotesControllerTest
{
    private readonly Mock<INoteSearcher> source = new Mock<INoteSearcher>();
    private readonly SearchNotesController sut = new();
    private readonly LocalDate date = new LocalDate(1975, 07, 28);

    [Test]
    public async Task SimpleSearch()
    {
        source.Setup(i => i.SearchFor("foo", date, date)).Returns(new[]
        {
            new NoteTitle{Title ="Title", Key =Guid.Empty, Date = date}
        }.ToAsyncEnumerable);

        Assert.Equal("Title", (await sut.Search("foo", date, date, source.Object).FirstAsync()).Title);
            
    }



}