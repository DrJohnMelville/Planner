using System.Text.RegularExpressions;
using System.Xml.Schema;
using Markdig;
using NodaTime;
using Planner.Models.Markdown.PlannerLinks;
namespace Planner.Models.Markdown
{
    public interface IMarkdownTranslator
    {
        public string Render(string markdown);
        public string RenderLine(string markdown);
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

        public string Render(string markdown) => Markdig.Markdown.ToHtml(markdown, translator);
        
        private static readonly Regex paragraphFinder = new Regex(@"\</?p\>");
        public string RenderLine(string markdown) => 
            paragraphFinder.Replace(Render(markdown), "");
    }
    
    
}