using Markdig.Helpers;
using Markdig.Syntax.Inlines;
using NodaTime;

namespace Planner.Models.Markdown.PlannerLinks
{
    public class PlannerLink : LeafInline
    {
        public LocalDate Target { get; init; }
        public StringSlice LiteralString { get; init; }
        public string PageLinkBase { get; init; } = "";
    }
}