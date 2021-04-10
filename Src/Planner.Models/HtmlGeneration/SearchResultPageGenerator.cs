using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Planner.Models.Notes;
using Planner.Models.Repositories;

namespace Planner.Models.HtmlGeneration
{
    public class SearchResultPageGenerator : TryNoteHtmlGenerator
    {
        private readonly ILocalRepository<Note> notesRepository; 
        private readonly Func<TextWriter, IJournalItemRenderer> rendererFactory;
        public SearchResultPageGenerator(ILocalRepository<Note> notesRepository, 
            Func<TextWriter, IJournalItemRenderer> rendererFactory) : base(
            new Regex("^List.*", RegexOptions.Singleline))
        {
            this.notesRepository = notesRepository;
            this.rendererFactory = rendererFactory;
        }

        private static readonly Regex guidFinder =
            new("[0-9a-f]{8}(?:-[0-9a-f]{4}){3}-[0-9a-f]{12}", RegexOptions.IgnoreCase);
        protected override async Task? TryRespond(Match match, Stream destination)
        {
            var guids = guidFinder.Matches(match.Value).Select(i => Guid.Parse((string) i.Value));
            var notes = (await notesRepository.ItemsByKeys(guids).CompleteList())
                .OrderBy(i=>i.Date).ThenBy(i=>i.TimeCreated);
            await using var writer = new StreamWriter(destination);
            rendererFactory(writer).WriteJournalList(notes.ToList(), (_,n)=>n.Date.ToString("d",null));
        }
    }
}