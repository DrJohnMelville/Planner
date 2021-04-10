using Markdig;
using Markdig.Renderers;

namespace Planner.Models.Markdown.PlannerLinks
{
    public class PlannerLinkExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.InlineParsers.Contains<PlannerLinkParser>())
            {
                pipeline.InlineParsers.Add(new PlannerLinkParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer &&
                htmlRenderer.ObjectRenderers is {} objRenderer &&
                !objRenderer.Contains<PlannerLinkRenderer>())
            {
                objRenderer.Add(new PlannerLinkRenderer());
            }
        }
    }
}