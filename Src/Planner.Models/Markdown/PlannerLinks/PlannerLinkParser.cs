using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using Markdig;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax.Inlines;
using Microsoft.VisualBasic;
using NodaTime;
using Planner.Models.HtmlGeneration;

namespace Planner.Models.Markdown.PlannerLinks
{
    public static class PlannerLinkExtensionOperations {
        public static MarkdownPipelineBuilder UsePlannerLinks(
            this MarkdownPipelineBuilder builder, LocalDate date)
        {
            if (!builder.Extensions.Contains<PlannerLinkExtension>())
            {
                builder.Extensions.Add(new PlannerLinkExtension(date));
            }

            return builder;
        }
    }
    public class PlannerLinkExtension : IMarkdownExtension
    {
        private readonly LocalDate baseDate;

        public PlannerLinkExtension(LocalDate baseDate)
        {
            this.baseDate = baseDate;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.InlineParsers.Contains<PlannerLinkParser>())
            {
                pipeline.InlineParsers.Add(new PlannerLinkParser(baseDate));
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
    public class PlannerLink : LeafInline
    {
        public LocalDate Target { get; set; }
        public StringSlice LiteralString { get; set; }
    }

    public class PlannerLinkRenderer : HtmlObjectRenderer<PlannerLink>
    {
        protected override void Write(HtmlRenderer renderer, PlannerLink obj)
        {
            WriteIfHtmlActive(renderer, $"<a href='/navToPage/{obj.Target:yyyy-M-d}'>");
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

    public class PlannerLinkParser: InlineParser
    {
        private LocalDate baseDate;
        public PlannerLinkParser(LocalDate baseDate)
        {
            this.baseDate = baseDate;
            OpeningCharacters = new[]{'('};
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            int start = slice.Start;
            Debug.Assert(slice.CurrentChar == '(');
            slice.NextChar();
            
            var ints = new List<int>();
            while (char.IsDigit(slice.CurrentChar))
            {
                ints.Add(GetInteger(ref slice));
                TrySkipPeriods(ref slice);
            }

            int end = slice.Start;
            var inlineStart = processor.GetSourcePosition(slice.Start, out int line, out int column);
            if (slice.CurrentChar != ')') return false;
            slice.NextChar();
            LocalDate? date = ints.Count switch
            {
                3 => ContextualDateParser.SelectedDate(-1, ints[0], ints[1], baseDate),
                4 => ContextualDateParser.SelectedDate(ints[2], ints[0], ints[1], baseDate),
                _=> null
            };
            if (!date.HasValue) return false;
            processor.Inline = new PlannerLink()
            {
                Target = date.Value,
                LiteralString = new StringSlice(slice.Text, start, end),
                Span =
                {
                    Start = inlineStart,
                    End = inlineStart + (end - start) + 1
                },
                Line = line,
                Column = column
            };
            return true;
        }

        private static void TrySkipPeriods(ref StringSlice slice)
        {
            if (slice.CurrentChar == '.') slice.NextChar();
        }

        private int GetInteger(ref StringSlice slice)
        {
            Debug.Assert(slice.CurrentChar.IsDigit());
            int ret = 0;
            do
            {
                ret *= 10;
                ret += slice.CurrentChar - '0';
            } while (slice.NextChar().IsDigit());

            return ret;
        }
    }
}