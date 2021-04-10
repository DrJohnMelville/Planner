using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using Markdig;
using Markdig.Helpers;
using Markdig.Parsers;
using NodaTime;
using Planner.Models.HtmlGeneration;

namespace Planner.Models.Markdown.PlannerLinks
{
    public class PlannerLinkParser : InlineParser
    {
        public PlannerLinkParser()
        {
            OpeningCharacters = new[] {'('};
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            return processor.Context != null &&
                   new ParserImplementation(processor, processor.Context).Match(ref slice);
        }

        private ref struct ParserImplementation
        {
            private readonly InlineProcessor processor;
            private readonly MarkdownParserContext context;
            private int start;
            private int end;
            private int line;
            private int column;
            private int inlineStart;

            public ParserImplementation(InlineProcessor processor, MarkdownParserContext context)
            {
                this.processor = processor;
                this.context = context;

                start = -1;
                end = -1;
                line = -1;
                column = -1;
                inlineStart = -1;
            }

            public bool Match(ref StringSlice slice)
            {
                CheckForInitialParen(ref slice);
                var date = ComputeDateFromInts(GatherInput(ref slice));
                if (!CheckForClosingParen(ref slice)) return false;
                if (!date.HasValue) return false;
                CreateOutputLink(slice, date.Value);
                return true;
            }

            private void CheckForInitialParen(ref StringSlice slice)
            {
                start = slice.Start;
                Debug.Assert(slice.CurrentChar == '(');
                slice.NextChar();
            }

            private static bool CheckForClosingParen(ref StringSlice slice)
            {
                if (slice.CurrentChar != ')') return false;
                slice.NextChar();
                return true;
            }

            private IReadOnlyList<int> GatherInput(ref StringSlice slice)
            {
                var ints = CollectDottedIntList(ref slice);
                end = slice.Start;
                inlineStart = processor.GetSourcePosition(slice.Start, out line, out column);
                return ints;
            }

            private void CreateOutputLink(StringSlice slice, LocalDate localDate)
            {
                processor.Inline = new PlannerLink()
                {
                    Target = localDate,
                    LiteralString = new StringSlice(slice.Text, start, end),
                    PageLinkBase = context.GetDailyPageRoot(),
                    Span =
                    {
                        Start = inlineStart,
                        End = inlineStart + (end - start) + 1
                    },
                    Line = line,
                    Column = column
                };
            }

            private LocalDate? ComputeDateFromInts(IReadOnlyList<int> ints)
            {
                LocalDate? date1 = ints.Count switch
                {
                    3 => ContextualDateParser.SelectedDate(-1, ints[0], ints[1],
                        context.GetNoteDate()),
                    4 => ContextualDateParser.SelectedDate(ints[2], ints[0],
                        ints[1], context.GetNoteDate()),
                    _ => null
                };
                return date1;
            }

            private List<int> CollectDottedIntList(ref StringSlice slice)
            {
                var ints = new List<int>();
                while (char.IsDigit(slice.CurrentChar))
                {
                    ints.Add(GetInteger(ref slice));
                    if (slice.CurrentChar == '.') slice.NextChar();
                }
                return ints;
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
}