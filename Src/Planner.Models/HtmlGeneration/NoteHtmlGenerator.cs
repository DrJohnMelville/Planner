using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.Blobs;
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
        private readonly IBlobReader blobReader;
        public readonly Func<TextWriter, JournalItemRenderer> rendererFactory;
        
        public NoteHtmlGenerator(
            ILocalRepository<Note> noteRepo, Func<TextWriter, 
            JournalItemRenderer> rendererFactory,
            IStaticFiles staticFiles, 
            IBlobReader blobReader)
        {
            this.noteRepo = noteRepo;
            this.rendererFactory = rendererFactory;
            this.staticFiles = staticFiles;
            this.blobReader = blobReader;
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
                   ImageAsset(url, destination) ??
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

        private static Regex imageFinder = new Regex(@"([\d-]+)\/(\d+)\.(\d+)(?:\.(\d+))?_(\d+)");
        private Task? ImageAsset(string url, Stream destination)
        {
            var match = imageFinder.Match(url);
            if (!match.Success) return null;
            if (!TryParseLocalDate(match.Groups[1].Value, out var pageDate)) return null;

            return CopyBlob(
                ConstructDateFromMatch(pageDate, match.Groups), 
                ExtractOrdinalFromMatch(match), destination);
        }

        private static int ExtractOrdinalFromMatch(Match match)
        {
            return int.Parse(match.Groups[5].Value);
        }

        private static LocalDate ConstructDateFromMatch(LocalDate pageDate, GroupCollection groups) =>
            ContextualDateParser.SelectedDate(groups[4].Value, groups[2].Value, groups[3].Value,
                pageDate);

        private async Task CopyBlob(LocalDate date, int ordinal, Stream destination)
        {
            using var data = await blobReader.Read(date, ordinal);
            await data.CopyToAsync(destination);
        }

        private Task? DailyJournalPage(string url, TextWriter destnation) => 
            TryParseLocalDate(url[..^1], out var date) ? RenderJournalPage(destnation, date) : null;

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