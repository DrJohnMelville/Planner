using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Melville.MVVM.Functional;

namespace Planner.Models.Tasks
{
    public class SegmentDecl<T>
    {
        private readonly Regex recognizer;
        private readonly T label;
        private Regex formatter;

        public SegmentDecl(Regex recognizer, T label, Regex formatter)
        {
            this.recognizer = recognizer;
            this.label = label;
            this.formatter = formatter;
        }

        public Segment<T>? TryMatchSegment(string text, int startPos)
        {
            var match = recognizer.Match(text, startPos);
            return match.Success ? new Segment<T>(match.Value, 
                formatter.Match(match.Value).Groups[1].Value, label, match) : null;
        }
    }

    public class Segment<T>
    {
        public string Text { get; }
        public string DisplayText { get; }
        public int StartPos { get; }
        public int Length => Text.Length;
        public int NextPos => StartPos+Length;
        public T Label { get; }
        public Match? Match { get; }

        public Segment(string text, T label, int startPos)
        {
            Text = text;
            DisplayText = text;
            Label = label;
            StartPos = startPos;
        }
        public Segment(string text, string displayText, T label, Match match): this (text, label, match.Index)
        {
            Match = match;
            DisplayText = displayText;
        }
    }

    public class StringSegmenter<T>
    {
        private SegmentDecl<T>[] declarations;
        private readonly T defaultLabel;
        public StringSegmenter(SegmentDecl<T>[] declarations, T defaultLabel)
        {
            this.declarations = declarations;
            this.defaultLabel = defaultLabel;
        }

        public IEnumerable<Segment<T>> Parse(string text)
        {
            int startPos = 0;
            while (startPos < text.Length)
            {
                var nextLink = FindNextMatch(text, startPos);
                
                if (HasTextBeforeNextLink(startPos, nextLink.StartPos))
                    yield return SegmentBeforeLink(text, startPos, nextLink.StartPos);

                yield return nextLink;
                
                startPos = nextLink.NextPos;
            }
        }

        private Segment<T> SegmentBeforeLink(string text, int startPos, int linkStartPos) => 
            new Segment<T>(text[startPos..linkStartPos], defaultLabel, startPos);

        private static bool HasTextBeforeNextLink(int startPos, int linkPos) => linkPos > startPos;

        private Segment<T> FindNextMatch(string text, int startPos)
        {
            var candidates = CandidateLinkQuery(text, startPos);
            if (!candidates.Any())
            {
                return MatchRestOfString(text, startPos);
            }
            var nextLink = candidates.MinThat(i => i.StartPos);
            return nextLink;
        }

        private Segment<T> MatchRestOfString(string text, int startPos) =>
            new Segment<T>(text[startPos..], defaultLabel, startPos);

        private List<Segment<T>> CandidateLinkQuery(string text, int startPos) =>
            declarations
                .Select(i => i.TryMatchSegment(text, startPos))
                .OfType<Segment<T>>() // filters out the null segments
                .ToList();
    }
}