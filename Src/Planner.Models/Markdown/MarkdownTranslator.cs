using System.Text.RegularExpressions;
using System.Xml.Schema;
using Markdig;
using NodaTime;
using Planner.Models.Markdown.PlannerLinks;

namespace Planner.Models.Markdown
{
    public interface IMarkdownTranslator
    {
        public string Render(string markdown, LocalDate date);
        public string RenderLine(string markdown, LocalDate date);
    }
    public class MarkdownTranslator:IMarkdownTranslator
    {
        private readonly MarkdownPipeline translator;

        public MarkdownTranslator(LocalDate date)
        {
            translator =
                new MarkdownPipelineBuilder()
                    .UseAdvancedExtensions()
                    .UsePlannerLinks(date)
                    .Build();
        }

        public string Render(string markdown, LocalDate date) => Markdig.Markdown.ToHtml(markdown, translator);
        
        private static readonly Regex paragraphFinder = new Regex(@"\</?p\>");
        public string RenderLine(string markdown, LocalDate date) => 
            paragraphFinder.Replace(Render(markdown, date), "");
    }
    
    
}