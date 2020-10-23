using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Planner.Models.HtmlGeneration
{
    public class DefaultTextGenerator : TryNoteHtmlGenerator
    {
        public DefaultTextGenerator():base(new Regex(".*")) // matches everything because this is the default
        {
        }

        protected override Task? TryRespond(Match match, Stream destination) => 
            destination.WriteAsync(Encoding.UTF8.GetBytes("<html><body></body></html>")).AsTask();
    }
}