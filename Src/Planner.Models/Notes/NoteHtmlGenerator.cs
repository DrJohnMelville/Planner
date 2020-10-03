using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.Repositories;

namespace Planner.Models.Notes
{
    public interface INoteHtmlGenerator
    {
        Task GenerateResponse(string url, TextWriter destination);
    }

    public class NoteHtmlGenerator : INoteHtmlGenerator
    {
        private readonly ILocalRepository<Note> noteRepo;

        public NoteHtmlGenerator(ILocalRepository<Note> noteRepo)
        {
            this.noteRepo = noteRepo;
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
            if (TryParseLocalDate(url, out var date)) return PlannerDayView(date, destination);
            destination.Write("<html><body></body></html>");
            return Task.CompletedTask;
        }

        private async Task PlannerDayView(LocalDate date, TextWriter destination)
        {
            destination.Write("<html><body>");
            var notes = await noteRepo.CompletedItemsForDate(date);
            int position = 1;
            foreach (var note in notes.OrderBy(i=>i.TimeCreated))
            {
                GenerateNote(note, position++, destination);
            }
            destination.Write("</body></html>");
        }

        private void GenerateNote(Note note, int itemNumber, TextWriter destination)
        {
            if (itemNumber > 1)
            {
                destination.Write("<hr/>");
            }
            destination.Write("<h3>");
            destination.Write(itemNumber);
            destination.Write(". ");
            destination.Write(note.Title);
            destination.Write("</h3>");
            destination.Write("<div>");
            destination.Write(note.Text);
            destination.Write("</div>");
        }
    }
}