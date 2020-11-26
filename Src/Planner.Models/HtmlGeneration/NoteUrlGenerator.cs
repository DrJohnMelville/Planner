using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime;
using Planner.Models.Notes;

namespace Planner.Models.HtmlGeneration
{
    public interface INotesServer
    {
        string BaseUrl { get; }
        void Launch();
    }

    public interface INoteUrlGenerator
    {
        string DailyUrl(LocalDate date);
        string EditNoteUrl(LocalDate date, Guid key);
        string EditNoteUrl(Note note) => EditNoteUrl(note.Date, note.Key);
        string ShowNoteUrl(LocalDate date, Guid key);
        string ShowNoteUrl(Note note) => ShowNoteUrl(note.Date, note.Key);
        string ArbitraryNoteView(IEnumerable<Guid> noteKeys);
    }
    public class NoteUrlGenerator:INoteUrlGenerator
    {
        private readonly INotesServer server;
        private int nonce;
        public NoteUrlGenerator(INotesServer server)
        {
            this.server = server;
        }

        private string Prefix() => server.BaseUrl + (nonce++)+"/";
        public string DailyUrl(LocalDate date) => Prefix() + date.ToString("yyyy-M-d/", null);

        public string EditNoteUrl(LocalDate date, Guid key) => DailyUrl(date)+key;
        public string ShowNoteUrl(LocalDate date, Guid key) => DailyUrl(date)+"show/"+key;

        public string ArbitraryNoteView(IEnumerable<Guid> noteKeys) =>
            string.Join("/", noteKeys.Select(i => i.ToString()).Prepend(Prefix() + "List"));

    }
}