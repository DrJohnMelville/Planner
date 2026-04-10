using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Notes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Planner.Models.Markdown;

public static class MarkdownGuideText {

    public static Note[] Notes { get; }  = [
        new Note() {
            Title = "Example Note",
            Text = "This is an example note used to demonstrate the Markdown Guide.",
        }
    ];
}

public partial class MarkdownGuide(IMarkdownTranslator translator): TryNoteHtmlGenerator(GuideUrl())
{
    protected async override Task? TryRespond(Match match, Stream destination)
    {
        await using var writer = new StreamWriter(destination);
        var generator = new JournalItemRenderer(writer, translator, new FakeUrlGenerator());
        generator.WriteJournalList([new Note() {
            Title = "Example Note",
            Text = "This is an example note used to demonstrate the **Markdown Guide**.",
        }], (i, n) => "Guide", null);

    }

    [GeneratedRegex(@"^Guide/?(\d*)")]
    private static partial Regex GuideUrl();
}

internal class FakeUrlGenerator : INoteUrlGenerator
{
    public string ArbitraryNoteView(IEnumerable<Guid> noteKeys) => "";

    public string CreateGuideUrl(int index) => "";

    public string DailyUrl(LocalDate date) => "";   

    public string EditNoteUrl(LocalDate date, Guid key) => "";

    public string ShowNoteUrl(LocalDate date, Guid key) => "";
}