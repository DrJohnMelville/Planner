using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

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

        public Segment(string text, T label)
        {
            Text = text;
            Label = label;
        }
    }

    public class StringSegmenter<T>
    {
        private SegmentDecl<T>[] declarations;

        public StringSegmenter(SegmentDecl<T>[] declarations)
        {
            this.declarations = declarations;
        }

        public IEnumerable<Segment<T>> Parse(string text, T defaultLabel)
        {
            yield return new Segment<T>(text, defaultLabel);
        }
    }

    public enum TaskTextType
    {
        NoLink
    }
    public class TaskNameParser
    {
        private StringSegmenter<TaskTextType> declarations = new StringSegmenter<TaskTextType>(
            new SegmentDecl<TaskTextType>[]
            {
            }
            );
        
        public IEnumerable<Segment<TaskTextType>> Parse(string text)
        {
            return declarations.Parse(text, TaskTextType.NoLink);
        }
    }
}