using Markdig;

namespace Planner.Models.Markdown.PlannerLinks
{
    public static class PlannerLinkExtensionOperations {
        public static MarkdownPipelineBuilder UsePlannerLinks(
            this MarkdownPipelineBuilder builder)
        {
            if (!builder.Extensions.Contains<PlannerLinkExtension>())
            {
                builder.Extensions.Add(new PlannerLinkExtension());
            }

            return builder;
        }
    }
}