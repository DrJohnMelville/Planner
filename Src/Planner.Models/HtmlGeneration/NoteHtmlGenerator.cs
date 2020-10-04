using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.Markdown;
using Planner.Models.Notes;
using Planner.Models.Repositories;

namespace Planner.Models.HtmlGeneration
{
    public interface INoteHtmlGenerator
    {
        Task GenerateResponse(string url, TextWriter destination);
    }
    public class NoteHtmlGenerator : INoteHtmlGenerator
    {
        private readonly ILocalRepository<Note> noteRepo;
        private readonly IStaticFiles staticFiles;
        public readonly Func<TextWriter, JournalItemRenderer> rendererFactory;

        public NoteHtmlGenerator(
            ILocalRepository<Note> noteRepo, Func<TextWriter, 
            JournalItemRenderer> rendererFactory,
            IStaticFiles staticFiles)
        {
            this.noteRepo = noteRepo;
            this.rendererFactory = rendererFactory;
            this.staticFiles = staticFiles;
        }

        private static bool TryParseLocalDate(string s, out LocalDate ret)
        {
            if (DateTime.TryParse(s, out var dt))
            {
                ret = LocalDate.FromDateTime(dt);
                return true;
            }
            ret = LocalDate.MinIsoValue;
            return false;
        }
        
        public Task GenerateResponse(string url, TextWriter destination)
        {
            if (staticFiles.TryGetValue(url, out var bytes))
            {
                destination.Write(Encoding.UTF8.GetString(bytes));
            }
            if (TryParseLocalDate(url, out var date)) return PlannerDayView(date, destination);
            destination.Write("<html><body></body></html>");
            return Task.CompletedTask;
        }

        private async Task PlannerDayView(LocalDate date, TextWriter destination) =>
            rendererFactory(destination).WriteJournalList(
                await noteRepo.CompletedItemsForDate(date));
    }
}