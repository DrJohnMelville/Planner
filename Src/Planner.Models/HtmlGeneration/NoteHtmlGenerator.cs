using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.Notes;
using Planner.Models.Repositories;

namespace Planner.Models.HtmlGeneration
{
    public class NoteEditRequestEventArgs : EventArgs
    {
        public IList<Note> DailyList { get; }
        public Note Note { get; }

        public NoteEditRequestEventArgs(IList<Note> dailyList, Note note)
        {
            DailyList = dailyList;
            Note = note;
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

        public async Task GenerateResponse(string url, Stream destination)
        {
            await using var writer = new StreamWriter(destination);
            await (StaticFiles(url, destination) ??
                   DailyJournalPage(url, writer) ??
                   EditRequestPage(url, writer) ??
                   DefaultText(writer));
        }

        private Task? StaticFiles(string url, Stream destination)
        {
            return staticFiles.TryGetValue(url, out var bytes) ? 
                destination.WriteAsync(bytes, 0, bytes.Length) 
                : null;

        }
        
        private Task? DailyJournalPage(string url, TextWriter destnation) => 
            TryParseLocalDate(url, out var date) ? RenderJournalPage(destnation, date) : null;

        private async Task RenderJournalPage(TextWriter writer, LocalDate date) =>
            rendererFactory(writer).WriteJournalList(
                await noteRepo.CompletedItemsForDate(date));

        private Task? EditRequestPage(string url, TextWriter destination)
        {
            var match = EditRequestFinder.Match(url);
            return match.Success &&
                   Guid.TryParse(match.Groups[2].Value, out var guid) &&
                   TryParseLocalDate(match.Groups[1].Value, out var dateTime)
                ? EditRequestPage(dateTime, guid, destination)
                : null;
        }

        private async Task EditRequestPage(LocalDate date, Guid noteKey, TextWriter destination)
        {
            var list = await noteRepo.CompletedItemsForDate(date);
            var item = list.FirstOrDefault(i => i.Key == noteKey);
            if (item == null) return;
            
            NoteEditRequested?.Invoke(this,  new NoteEditRequestEventArgs(list, item));
            rendererFactory(destination).WriteJournalList(list, item);
        }
        
        private Task DefaultText(TextWriter writer) =>writer.WriteAsync("<html><body></body></html>");
        
        public event EventHandler<NoteEditRequestEventArgs>? NoteEditRequested;
        //pattern for url is 9999-1-1/00000000-0000-0000-0000-000000000000
        private readonly static Regex EditRequestFinder = new Regex(
            @"(\d{4}-\d{1,2}-\d{1,2})/([0-9a-f]{8}(?:-[0-9a-f]{4}){3}-[0-9a-f]{12})");
    }
}