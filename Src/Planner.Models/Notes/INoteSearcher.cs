using System;
using System.Collections.Generic;
using NodaTime;

namespace Planner.Models.Notes
{
    public record NoteTitle(string Title, Guid Key, LocalDate Date)
    {
        
    }

    public interface INoteSearcher
    {
        IAsyncEnumerable<NoteTitle> SearchFor(string query, LocalDate minDate, LocalDate maxDate);
    }
}