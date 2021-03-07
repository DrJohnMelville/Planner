using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Planner.Models.Notes;

namespace Planner.Repository.SqLite
{
    public class SqlNoteSearcher: INoteSearcher
    {
        private readonly Func<PlannerDataContext> contextFactory;

        public SqlNoteSearcher(Func<PlannerDataContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public IAsyncEnumerable<NoteTitle> SearchFor(string query, LocalDate minDate, LocalDate maxDate)
        {
            var ctx = contextFactory();
            return new DisposeWithAsyncEnumerable<NoteTitle>(
                QueryDeclaration(query, minDate, maxDate, ctx), ctx);
        }

        private static IAsyncEnumerable<NoteTitle> QueryDeclaration(
            string query, LocalDate minDate, LocalDate maxDate, PlannerDataContext ctx) =>
            ctx.Notes
                .FromSqlRaw(
                    "select * from Notes where ((Title like {0} collate NOCASE) or (Text like {0} collate NOCASE))",
                    $"%{query}%")
                .Where(i=>i.Date >= minDate && i.Date <= maxDate)
                .OrderByDescending(i => i.Date).ThenBy(i => i.TimeCreated)
                .Take(300)
                .Select(i => new NoteTitle{Title = i.Title, Key = i.Key, Date = i.Date})
                .AsAsyncEnumerable();
    }
}