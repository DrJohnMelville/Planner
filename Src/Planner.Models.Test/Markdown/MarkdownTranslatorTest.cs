using NodaTime;
using Planner.Models.Markdown;
using Xunit;

namespace Planner.Models.Test.Markdown
{
    public class MarkdownTranslatorTest
    {
        private readonly IMarkdownTranslator sut;
        private readonly LocalDate baseDate = new LocalDate(1975, 7, 28);

        public MarkdownTranslatorTest()
        {
            sut = new MarkdownTranslator("/PageRoot/");
        }

        [Theory]
        [InlineData("(3.2.1)", 1975, "/PageRoot/1975-3-2")]
        [InlineData("(3.2.1)", 1976, "/PageRoot/1976-3-2")]
        [InlineData("(3.2.80.1)", 1976, "/PageRoot/1980-3-2")]
        [InlineData("(3.2.2080.1)", 1976, "/PageRoot/2080-3-2")]
        public void DailyPageRootRespectsDailyPage(string link, int year, string output)
        {
            var ret = sut.Render(new LocalDate(year, 7, 28), link);
            Assert.Contains(output, ret);
        }

        [Fact]
        public void ImplicitParagraphs()
        {
            Assert.Equal("<p>Text</p>\n", sut.Render(baseDate, "Text"));
            Assert.Equal("Text", sut.RenderLine(baseDate, "Text"));
            
        }


    }
}