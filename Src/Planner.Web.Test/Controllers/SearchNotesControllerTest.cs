using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NodaTime;
using Planner.Models.Notes;
using Planner.Web.Controllers;
using Xunit;

namespace Planner.Web.Test.Controllers
{
    public class SearchNotesControllerTest
    {
        private readonly Mock<INoteSearcher> source = new Mock<INoteSearcher>();
        private readonly SearchNotesController sut = new();
        private readonly LocalDate date = new LocalDate(1975, 07, 28);

        [Fact]
        public async Task SimpleSearch()
        {
            source.Setup(i => i.SearchFor("foo", date, date)).Returns(new[]
            {
                new NoteTitle("Title", Guid.Empty, date)
            }.ToAsyncEnumerable);

            Assert.Equal("Title", (await sut.Search("foo", date, date, source.Object).FirstAsync()).Title);
            
        }



    }
}