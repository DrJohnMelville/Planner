using System;
using System.Collections.Generic;
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
        Task GenerateResponse(string url, Stream destination);
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
        
        public Task GenerateResponse(string url, Stream destination) => 
            staticFiles.TryGetValue(url, out var bytes) ?
                destination.WriteAsync(bytes, 0, bytes.Length) : 
                GenerateTextResponses(url, destination);

        private async Task GenerateTextResponses(string url, Stream destinatiom)
        {
            await using var writer = new StreamWriter(destinatiom);
            if (TryParseLocalDate(url, out var date))
                RenderItemsAsJournal(writer, await noteRepo.CompletedItemsForDate(date));
            else
                writer.Write("<html><body></body></html>");
        }

        private void RenderItemsAsJournal(StreamWriter writer, IList<Note> items) =>
            rendererFactory(writer).WriteJournalList(
                items);
    }
}