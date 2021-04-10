using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml.Schema;
using Markdig;
using Markdig.Parsers;
using Markdig.Renderers;
using NodaTime;
using Planner.Models.Markdown.PlannerLinks;
namespace Planner.Models.Markdown
{
    public interface IMarkdownTranslator
    {
        public string Render(LocalDate baseDate, string markdown, bool supressParagraph = false);
    }

    public static class MarkdownTranslatorOperations
    {
        public static string RenderLine(this IMarkdownTranslator translator, LocalDate date, string markdown) =>
            translator.Render(date, markdown, true);
    }
    public class MarkdownTranslator:IMarkdownTranslator
    {
        private readonly string dailyPageRoot;
        private readonly string imageRoot;
        private readonly MarkdownPipeline translator;

        public MarkdownTranslator(string dailyPageRoot, string imageRoot)
        {
            this.dailyPageRoot = dailyPageRoot;
            this.imageRoot = imageRoot;
            translator =
                new MarkdownPipelineBuilder()
                    .UseAdvancedExtensions()
                    .UsePlannerLinks()
                    .Build();
        }

        public string Render(LocalDate baseDate, string markdown, bool supressParagraph = false)
        {
            var writer = new StringWriter();
            RenderToTextWriter(markdown,
                ParserContext(baseDate), 
                CreateHtmlRenderer(writer, supressParagraph, baseDate));
            writer.Flush();
            return writer.ToString();
        }

        private void RenderToTextWriter(
            string markdown, MarkdownParserContext context, HtmlRenderer renderer) => 
            renderer
                .Render(MarkdownParser.Parse(markdown, translator, context));

        private HtmlRenderer CreateHtmlRenderer(StringWriter writer, bool supressParagraph, 
            LocalDate localDate)
        {

            var renderer = new HtmlRenderer(writer)
            {
                ImplicitParagraph = supressParagraph,
                LinkRewriter = LinkRewriter
            };
            translator.Setup(renderer);
            return renderer;

            string LinkRewriter(string s) => 
                Regex.IsMatch(s,@"^\d{1,2}\.\d{1,2}(?:.\d{2,4})?_\d+$") ? $"{imageRoot}{localDate:yyyy-M-d}/{s}":s;
        }

        private MarkdownParserContext ParserContext(LocalDate baseDate)
        {
            var context = new MarkdownParserContext();
            context.SetupCustomParserContext(baseDate, dailyPageRoot);
            return context;
        }
    }
    
    
}