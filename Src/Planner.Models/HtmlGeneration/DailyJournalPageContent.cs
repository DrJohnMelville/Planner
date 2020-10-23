using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Planner.Models.Time;

namespace Planner.Models.HtmlGeneration
{
    public class DailyJournalPageContent : HtmlContentOption
    {
        private readonly Func<TextWriter, JournalItemRenderer> rendererFactory;
        private ILocalRepository<Note> noteRepository;
        public DailyJournalPageContent(
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
            var items = await noteRepository.CompletedItemsForDate(date);
            rendererFactory(writer).WriteJournalList(items, items.FirstOrDefault(
                i=>note.HasValue && note == i.Key));
        }
    }
}