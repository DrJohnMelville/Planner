using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Planner.Models.Time;

namespace Planner.Models.HtmlGeneration
{
    public sealed class EditNoteNotificationGenerator : TryNoteHtmlGenerator
    {
        private readonly ILocalRepository<Note> noteRepo;
        private readonly IEventBroadcast<NoteEditRequestEventArgs> notifyEventRequest;

        public EditNoteNotificationGenerator(ILocalRepository<Note> noteRepo, IEventBroadcast<NoteEditRequestEventArgs> notifyEventRequest) : base(
            new Regex(@"(\d{4}-\d{1,2}-\d{1,2})/([0-9a-f]{8}(?:-[0-9a-f]{4}){3}-[0-9a-f]{12})"))
        {
            this.noteRepo = noteRepo;
            this.notifyEventRequest = notifyEventRequest;
        }

        protected override async Task? TryRespond(Match match, Stream destination)
        {
            if (!(TimeOperations.TryParseLocalDate(match.Groups[1].Value, out var date) &&
                  Guid.TryParse(match.Groups[2].Value, out var noteKey))) return;
            
            var list = await noteRepo.CompletedItemsForDate(date);
            var item = list.FirstOrDefault(i => i.Key == noteKey);
            if (item == null) return;
            
            notifyEventRequest.Fire(this,  new NoteEditRequestEventArgs(list, item));
  
        }
    }
}