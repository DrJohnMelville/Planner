using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Planner.Models.Markdown.PlannerLinks
{
    public class PlannerLinkRenderer : HtmlObjectRenderer<PlannerLink>
    {
        protected override void Write(HtmlRenderer renderer, PlannerLink obj)
        {
            WriteIfHtmlActive(renderer, $"<a href='{obj.PageLinkBase}{obj.Target:yyyy-M-d}'>");
            renderer.Write(obj.LiteralString);
            WriteIfHtmlActive(renderer, $"</a>");
        }

        private static void WriteIfHtmlActive(HtmlRenderer renderer, string text)
        {
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write(text);
            }
        }
    }
}