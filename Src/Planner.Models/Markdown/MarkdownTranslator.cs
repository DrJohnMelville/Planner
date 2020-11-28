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
        private readonly MarkdownPipeline translator;

        public MarkdownTranslator(string dailyPageRoot)
        {
            this.dailyPageRoot = dailyPageRoot;
            translator =
                new MarkdownPipelineBuilder()
                    .UseAdvancedExtensions()
                    .UsePlannerLinks()
                    .Build();
        }

        public string Render(LocalDate baseDate, string markdown, bool supressParagraph = false)
        {
            var (writer, renderer) = CreateRenderer(supressParagraph);
            renderer.Render(MarkdownParser.Parse(markdown, translator, ParserContext(baseDate)));
            writer.Flush();
            return writer.ToString();
        }

        private MarkdownParserContext ParserContext(LocalDate baseDate)
        {
            var context = new MarkdownParserContext();
            context.Properties.Add(PlannerLinkParser.NoteDateKey, baseDate);
            context.Properties.Add(PlannerLinkParser.DailyPageRootKey, dailyPageRoot);
            return context;
        }

        private (StringWriter writer, HtmlRenderer renderer) CreateRenderer(bool supressParagraph)
        {
            var writer = new StringWriter();
            var renderer = new HtmlRenderer(writer)
            {
                ImplicitParagraph = supressParagraph
            };
            translator.Setup(renderer);
            return (writer, renderer);
        }
    }
    
    
}