using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Planner.Models.Notes;
using Planner.Models.Repositories;

namespace Planner.Models.HtmlGeneration
{
    public partial class SearchResultPageGenerator : TryNoteHtmlGenerator
    {
        private readonly ILocalRepository<Note> notesRepository; 
        private readonly Func<TextWriter, IJournalItemRenderer> rendererFactory;
        public SearchResultPageGenerator(ILocalRepository<Note> notesRepository, 
            Func<TextWriter, IJournalItemRenderer> rendererFactory) : base(
            ListFilter())
        {
            this.notesRepository = notesRepository;
            this.rendererFactory = rendererFactory;
        }

        protected override async Task? TryRespond(Match match, Stream destination)
        {
            var guids = GuidFinder().Matches(match.Value).Select(i => Guid.Parse((string) i.Value));
            var notes = (await notesRepository.ItemsByKeys(guids).CompleteList())
                .OrderBy(i=>i.Date).ThenBy(i=>i.TimeCreated);
            await using var writer = new StreamWriter(destination);
            rendererFactory(writer).WriteJournalList(notes.ToList(), (_,n)=>n.Date.ToString("d",null));
        }

        [GeneratedRegex("^List.*", RegexOptions.Singleline)]
        private static partial Regex ListFilter();
        [GeneratedRegex("[0-9a-f]{8}(?:-[0-9a-f]{4}){3}-[0-9a-f]{12}", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex GuidFinder();
    }
}