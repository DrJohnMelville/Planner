using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Melville.MVVM.Functional;

namespace Planner.Models.Tasks
{
    public class SegmentDecl<T>
    {
        public Regex Recognizer { get; }
        public T Label {get;}

        public SegmentDecl(Regex recognizer, T label)
        {
            Recognizer = recognizer;
            Label = label;
        }
    }

    public class Segment<T>
    {
        public string Text { get; }
        public T Label { get; }
        public Match? Match { get; }

        public Segment(string text, T label, Match? match)
        {
            Text = text;
            Label = label;
            Match = match;
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

                yield return LinkAsSegment(text, nextLink);
                
                startPos = nextLink.NextPos;
            }
        }

        private static Segment<T> LinkAsSegment(string text, 
            (Match? Match, T label, int StartPos, int NextPos) nextLink) =>
            new Segment<T>(text[nextLink.StartPos..nextLink.NextPos], 
                nextLink.label, nextLink.Match);

        private Segment<T> SegmentBeforeLink(string text, int startPos, int linkStartPos) => 
            new Segment<T>(text[startPos..linkStartPos], defaultLabel, null);

        private static bool HasTextBeforeNextLink(int startPos, int linkPos) => linkPos > startPos;

        private (Match? Match, T label, int StartPos, int NextPos) FindNextMatch(string text, int startPos)
        {
            var candidates = CandidateLinkQuery(text, startPos);
            if (!candidates.Any())
            {
                return MatchRestOfString(text, startPos);
            }
            var nextLink = candidates.MinThat(i => i.Match.Index);
            return LinkToMatchData(nextLink);
        }

        private (Match? Match, T label, int StartPos, int NextPos) 
            MatchRestOfString(string text, int startPos) => 
            (null, defaultLabel, startPos, text.Length);

        private static (Match Match, T Label, int Index, int) 
            LinkToMatchData((Match Match, T Label) nextLink) =>
            (nextLink.Match, nextLink.Label,
                nextLink.Match.Index, nextLink.Match.Index + nextLink.Match.Length);

        private List<(Match Match, T Label)> CandidateLinkQuery(string text, int startPos) =>
            declarations
                .Select(i => (Match:i.Recognizer.Match(text, startPos), Label:i.Label))
                .Where(i => i.Match.Success)
                .ToList();
    }
}