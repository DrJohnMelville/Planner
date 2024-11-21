using NodaTime;
using Planner.Models.Markdown;

namespace Planner.Test.Models.Markdown;

public class MarkdownTranslatorTest
{
    private readonly IMarkdownTranslator sut;
    private readonly LocalDate baseDate = new LocalDate(1975, 7, 28);

    public MarkdownTranslatorTest()
    {
        sut = new MarkdownTranslator("/PageRoot/", "/ImageBase/");
    }

    [Test]
    [Arguments("(3.2.1)", 1975, "/PageRoot/1975-3-2")]
    [Arguments("(3.2.1)", 1976, "/PageRoot/1976-3-2")]
    [Arguments("(3.2.80.1)", 1976, "/PageRoot/1980-3-2")]
    [Arguments("(3.2.2080.1)", 1976, "/PageRoot/2080-3-2")]
    public void DailyPageRootRespectsDailyPage(string link, int year, string output) => 
        Xunit.Assert.Contains(output, sut.Render(new LocalDate(year, 7, 28), link));

    [Test]
    public void ImplicitParagraphs()
    {
        Xunit.Assert.Equal("<p>Text</p>\n", sut.Render(baseDate, "Text"));
        Xunit.Assert.Equal("Text", sut.RenderLine(baseDate, "Text"));
    }

    [Test]
    public void RenderImageLink() =>
        Xunit.Assert.Equal("<img src=\"/ImageBase/1975-7-28/7.28_1\" alt=\"Alt Text\" />", 
            sut.RenderLine(baseDate, "![Alt Text](7.28_1)"));
}
