using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
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
    }
    
    public class NoteHtmlGenerator : INoteHtmlGenerator
    {
        private readonly IList<IHtmlContentOption> options;
        private readonly ILocalRepository<Note> noteRepo;
        private readonly IEventBroadcast<NoteEditRequestEventArgs> notifyEventRequest;

        public NoteHtmlGenerator(
            ILocalRepository<Note> noteRepo, 
            IList<IHtmlContentOption> options, IEventBroadcast<NoteEditRequestEventArgs> notifyEventRequest)
        {
            this.noteRepo = noteRepo;
            this.options = options;
            this.notifyEventRequest = notifyEventRequest;
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

        public Task GenerateResponse(string url, Stream destination)
        {
            return (EditRequestPage(url) ??  TryOptions(url, destination));
        }

        private Task TryOptions(string url, Stream destination) =>
            options
                .Select(i => i.TryRespond(url, destination))
                .FirstOrDefault(i => i != null) 
                ?? Task.CompletedTask;

        
        private Task? EditRequestPage(string url)
        {
            var match = EditRequestFinder.Match(url);
            return match.Success &&
                   Guid.TryParse(match.Groups[2].Value, out var guid) &&
                   TryParseLocalDate(match.Groups[1].Value, out var dateTime)
                ? EditRequestPage(dateTime, guid)
                : null;
        }

        private async Task EditRequestPage(LocalDate date, Guid noteKey)
        {
            var list = await noteRepo.CompletedItemsForDate(date);
            var item = list.FirstOrDefault(i => i.Key == noteKey);
            if (item == null) return;
            
            notifyEventRequest.Fire(this,  new NoteEditRequestEventArgs(list, item));
        }
        //pattern for url is 9999-1-1/00000000-0000-0000-0000-000000000000
        private readonly static Regex EditRequestFinder = new Regex(
            @"(\d{4}-\d{1,2}-\d{1,2})/([0-9a-f]{8}(?:-[0-9a-f]{4}){3}-[0-9a-f]{12})");
    }
}