using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Planner.Models.HtmlGeneration
{
    public interface ITryNoteHtmlGenerator
    {
        Task? TryRespond(string url, Stream destination);
    }

    public abstract class TryNoteHtmlGenerator:ITryNoteHtmlGenerator
    {
        private readonly Regex filter;

        protected TryNoteHtmlGenerator(Regex filter)
        {
            this.filter = filter;
        }

        protected abstract Task? TryRespond(Match match, Stream destination);

        public Task? TryRespond(string url, Stream destination)
        {
            var match = filter.Match(url);
            return match.Success ? TryRespond(match, destination) : null;
        }
    }
}