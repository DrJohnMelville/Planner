using System;
using System.Collections.Generic;
using NodaTime;

namespace Planner.Models.Notes
{
    public class NoteTitle
    {
        public string Title { get; set; } = "";
        public Guid Key { get; set; }
        public LocalDate Date { get; set; }
    }

    public interface INoteSearcher
    {
        IAsyncEnumerable<NoteTitle> SearchFor(string query, LocalDate minDate, LocalDate maxDate);
    }
}