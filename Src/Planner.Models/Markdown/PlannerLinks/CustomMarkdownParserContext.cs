using Markdig;
using NodaTime;

namespace Planner.Models.Markdown.PlannerLinks
{
    public static class CustomMarkdownParserContext
    {
        private static readonly object NoteDateKey = new object();
        public static LocalDate GetNoteDate(this MarkdownParserContext context) =>
            (LocalDate) context.Properties[NoteDateKey];

        private static readonly object DailyPageRootKey = new object();
        public static string GetDailyPageRoot(this MarkdownParserContext context) =>
            (string) context.Properties[DailyPageRootKey];

        public static void SetupCustomParserContext(
            this MarkdownParserContext context, LocalDate date, string dailyPageRoot)
        {
            context.Properties[NoteDateKey] = date;
            context.Properties[DailyPageRootKey] = dailyPageRoot;
        }
    }
}