using System.Text.RegularExpressions;
using System.Xml.Schema;
using Markdig;

namespace Planner.Models.Markdown
{
    public interface IMarkdownTranslator
    {
        public string Render(string markdown);
        public string RenderLine(string markdown);
    }
    public class MarkdownTranslator:IMarkdownTranslator
    {
        private readonly MarkdownPipeline translator =
            new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        public string Render(string markdown) => Markdig.Markdown.ToHtml(markdown, translator);
        
        private static readonly Regex paragraphFinder = new Regex(@"\</?p\>");
        public string RenderLine(string markdown) => paragraphFinder.Replace(Render(markdown), "");
    }
}