using System.Text.RegularExpressions;
using System.Xml.Schema;
using Markdig;
using NodaTime;
using Planner.Models.Markdown.PlannerLinks;
namespace Planner.Models.Markdown
{
    public interface IMarkdownTranslator
    {
        public string Render(LocalDate baseDate, string markdown);
    }

    public static class MarkdownTranslatorOperations
    {
        private static readonly Regex paragraphFinder = new Regex(@"\</?p\>");

        public static string RenderLine(this IMarkdownTranslator translator, LocalDate date, string markdown) =>
            paragraphFinder.Replace(translator.Render(date, markdown), "");
    }
    public class MarkdownTranslator:IMarkdownTranslator
    {
        private readonly MarkdownPipeline translator;
        private LocalDate baseDate = LocalDate.MinIsoValue;

        public MarkdownTranslator(string dailyPageRoot)
        {
            translator =
                new MarkdownPipelineBuilder()
                    .UseAdvancedExtensions()
                    .UsePlannerLinks(()=>baseDate, dailyPageRoot)
                    .Build();
        }

        public string Render(LocalDate baseDate, string markdown)
        {
            this.baseDate = baseDate;
            return Markdig.Markdown.ToHtml(markdown, translator);
        }

    }
    
    
}