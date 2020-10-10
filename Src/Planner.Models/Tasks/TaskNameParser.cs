using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace Planner.Models.Tasks
{

    public enum TaskTextType
    {
        NoLink,
        PlannerPage,
        WebLink,
        FileLink
    }
    public class TaskNameParser
    {
        private StringSegmenter<TaskTextType> declarations = new StringSegmenter<TaskTextType>(
            new SegmentDecl<TaskTextType>[]
            {
                new (new Regex(@"\((\d+)\.(\d+)\.(\d+)\)"), TaskTextType.PlannerPage), 
                new (new Regex(@"\((\d+)\.(\d+)\.(\d+).(\d+)\)"), TaskTextType.PlannerPage), 
                new (new Regex(@"(?i)\b((?:[a-z][\w-]+:(?:/{1,3}|[a-z0-9%])|www\d{0,3}[.]|[a-z0-9.\-]+[.][a-z]{2,4}/)(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[\]{};:'"".,<>?«»“”‘’]))"),
                    TaskTextType.WebLink), 
                new (new Regex(@"(?i)([a-z]\:\\\S+)"), TaskTextType.FileLink),
                new (new Regex(@"(?i)""([a-z]\:\\[^""]+)"""), TaskTextType.FileLink),
                new (new Regex(@"\\\\\w+\\\S+"), TaskTextType.FileLink),
                new (new Regex(@"""\\\\\w+\\[^""]+"""), TaskTextType.FileLink),
            }, TaskTextType.NoLink
            );
        
        public IEnumerable<Segment<TaskTextType>> Parse(string text)
        {
            return declarations.Parse(text);
        }
    }
}