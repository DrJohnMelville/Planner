using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Planner.Models.Time;

namespace Planner.Models.HtmlGeneration
{
    public class DailyJournalPageGenerator : TryNoteHtmlGenerator
    {
        private readonly Func<TextWriter, JournalItemRenderer> rendererFactory;
        private ILocalRepository<Note> noteRepository;
        public DailyJournalPageGenerator(
            Func<TextWriter, JournalItemRenderer> rendererFactory, 
            ILocalRepository<Note> noteRepository) : base(
            new Regex(@"(\d{4}-\d{1,2}-\d{1,2})/(?:show/([0-9a-f]{8}(?:-[0-9a-f]{4}){3}-[0-9a-f]{12}))?"))
        {
            this.rendererFactory = rendererFactory;
            this.noteRepository = noteRepository;
        }

        protected override Task? TryRespond(Match match, Stream destination) =>
            TimeOperations.TryParseLocalDate(match.Groups[1].Value, out var date) ?
                TryRespond(date, TryParseGuid(match.Groups[2].Value), destination) : 
                null;

        private Guid? TryParseGuid(string value) => 
            (!string.IsNullOrWhiteSpace(value) && Guid.TryParse(value, out var ret)) ? (Guid?)ret : null;

        private async Task TryRespond(LocalDate date, Guid? note, Stream destination)
        {
            await using var writer = new StreamWriter(destination);
            var items = await noteRepository.ItemsForDate(date).CompleteList();
            rendererFactory(writer).WriteJournalList(items, items.FirstOrDefault(
                i=>note.HasValue && note == i.Key));
        }
    }

    public class SearchResultPageGenerator : TryNoteHtmlGenerator
    {
        public SearchResultPageGenerator() : base(
            new Regex("^List.*", RegexOptions.Singleline))
        {
        }

        private static readonly Regex guidFinder =
            new Regex("[0-9a-f]{8}(?:-[0-9a-f]{4}){3}-[0-9a-f]{12}");
        protected override Task? TryRespond(Match match, Stream destination)
        {
            var guids = guidFinder.Matches(match.Value).Select(i => Guid.Parse(i.Value));
            return destination.WriteAsync(Encoding.UTF8.GetBytes("Display Notes:"+
                string.Join("\r\n", guids))).AsTask();
        }
    }
}