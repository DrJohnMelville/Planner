using System;
using NodaTime;
using Planner.Models.Notes;

namespace Planner.Models.HtmlGeneration
{
    public interface INotesServer
    {
        string BaseUrl { get; }
        event EventHandler<NoteEditRequestEventArgs>? NoteEditRequested;
        void Launch();
    }

    public interface INoteUrlGenerator
    {
        string DailyUrl(LocalDate date);
        string EditNoteUrl(LocalDate date, Guid key);
        string EditNoteUrl(Note note) => EditNoteUrl(note.Date, note.Key);
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
    }
}