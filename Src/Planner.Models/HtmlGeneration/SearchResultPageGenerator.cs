using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Planner.Models.Notes;
using Planner.Models.Repositories;

namespace Planner.Models.HtmlGeneration
{
    public class SearchResultPageGenerator : TryNoteHtmlGenerator
    {
        private readonly ILocalRepository<Note> notesRepository; 
        public SearchResultPageGenerator(ILocalRepository<Note> notesRepository) : base(
            new Regex("^List.*", RegexOptions.Singleline))
        {
            this.notesRepository = notesRepository;
        }

        private static readonly Regex guidFinder =
            new("[0-9a-f]{8}(?:-[0-9a-f]{4}){3}-[0-9a-f]{12}", RegexOptions.IgnoreCase);
        protected override async Task? TryRespond(Match match, Stream destination)
        {
            var guids = guidFinder.Matches(match.Value).Select(i => Guid.Parse((string) i.Value));
            var notes = await notesRepository.ItemsByKeys(guids).CompleteList();
            await destination.WriteAsync(Encoding.UTF8.GetBytes("Display Notes:"+
               string.Join("\r\n", notes.Select(i=>i.Title)))).AsTask();
        }
    }
}