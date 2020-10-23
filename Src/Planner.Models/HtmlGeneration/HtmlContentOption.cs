using System.IO;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Planner.Models.HtmlGeneration
{
    public interface IHtmlContentOption
    {
        Task? TryRespond(string url, Stream destination);
    }

    public abstract class HtmlContentOption:IHtmlContentOption
    {
        private readonly Regex filter;

        protected HtmlContentOption(Regex filter)
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