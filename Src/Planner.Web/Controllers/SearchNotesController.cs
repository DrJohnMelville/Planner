using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Planner.Models.Notes;

namespace Planner.Web.Controllers
{
    [Route("SearchNotes")]
    public class SearchNotesController : Controller
    {
        [Route("{text}/{startDate}/{endDate}")]
        public IAsyncEnumerable<NoteTitle> Search(string text, LocalDate startDate, LocalDate endDate,
            [FromServices] INoteSearcher source) => source.SearchFor(text, startDate, endDate);
    }
}