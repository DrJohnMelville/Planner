using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.Markdown;
using Planner.Models.Notes;
using Planner.Models.Repositories;

namespace Planner.Models.HtmlGeneration
{
    
    public class NoteEditRequestEventArgs : EventArgs
    {
        public LocalDate Date { get; }
        public Guid NoteKey { get; }

        public NoteEditRequestEventArgs(LocalDate date, Guid noteKey)
        {
            Date = date;
            NoteKey = noteKey;
        }
    }

    public interface INoteHtmlGenerator
    {
        Task GenerateResponse(string url, Stream destination);
        event EventHandler<NoteEditRequestEventArgs>? NoteEditRequested;
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
            {
                RenderItemsAsJournal(writer, await noteRepo.CompletedItemsForDate(date));
                return;
            }

            CheckForEditRequest(url);
            writer.Write("<html><body></body></html>");
        }
        
        private void RenderItemsAsJournal(StreamWriter writer, IList<Note> items) =>
            rendererFactory(writer).WriteJournalList(
                items);

        public event EventHandler<NoteEditRequestEventArgs>? NoteEditRequested;
        //00000000-0000-0000-0000-000000000000/9999-1-1
        private readonly static Regex EditRequestFinder = new Regex(
            @"([0-9a-f]{8}(?:-[0-9a-f]{4}){3}-[0-9a-f]{12})/(\d{4}-\d{1,2}-\d{1,2})");
        private void CheckForEditRequest(string url)
        {
            var match = EditRequestFinder.Match(url);
            if (!(match.Success &&
                  Guid.TryParse(match.Groups[1].Value, out var guid) &&
                  DateTime.TryParse(match.Groups[2].Value, out var dateTime))) return;
            
            NoteEditRequested?.Invoke(this,
                new NoteEditRequestEventArgs(LocalDate.FromDateTime(dateTime), guid));
        }
    }
}