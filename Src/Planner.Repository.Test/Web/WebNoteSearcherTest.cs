using System.Net.Http;
using System.Threading.Tasks;
using Melville.TestHelpers.Http;
using Planner.Repository.Test.SqLite;
using Planner.Repository.Web;
using Xunit;

namespace Planner.Repository.Test.Web
{
    public class WebNoteSearcherTest : TestWithJsonWebService
    {
        private readonly WebNoteSearcher sut;

        public WebNoteSearcherTest(): base()
        {
            sut = new WebNoteSearcher(service);
        }

        [Fact]
        public async Task GetSearch()
        {
            httpSource.Setup(i=>i.EndsWith("SearchNotes/Foo/1975-07-28/1975-07-29"), HttpMethod.Get)
                .ReturnsJson("[{\"Title\":\"Title1\"}]");

            var items = await sut.SearchFor("Foo", date, date.PlusDays(1)).ToListAsync();
            Assert.Single(items);
            Assert.Equal("Title1", items[0].Title);
            
        }

    }
}