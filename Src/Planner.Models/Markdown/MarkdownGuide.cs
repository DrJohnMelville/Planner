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

public partial class MarkdownGuide(IMarkdownTranslator translator): TryNoteHtmlGenerator(GuideUrl())
{
    protected async override Task? TryRespond(Match match, Stream destination)
    {
        var writer = new StreamWriter(destination);

        await writer.WriteAsync("""<html><head><link rel="stylesheet" href="/0/journal.css"></head><body>""");
        await writer.WriteAsync(translator.Render(new LocalDate(), @"# Markdown Guide"));
        EpilogueManager.Write([], writer);
        writer.WriteLine("</body></html>");
    }

    [GeneratedRegex(@" ^ Guide/?(\d*)")]
    private static partial Regex GuideUrl();
}