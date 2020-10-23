using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Planner.Models.HtmlGeneration
{
    public interface INoteHtmlGenerator
    {
        Task GenerateResponse(string url, Stream destination);
    }
    
    public class NoteHtmlGenerator : INoteHtmlGenerator
    {
        private readonly IList<ITryNoteHtmlGenerator> options;

        public NoteHtmlGenerator(IList<ITryNoteHtmlGenerator> options)
        {
            this.options = options;
        }
        
        public Task GenerateResponse(string url, Stream destination)
        {
            return options
                       .Select(i => i.TryRespond(url, destination))
                       .FirstOrDefault(i => i != null) 
                   ?? Task.CompletedTask;
        }
    }
}