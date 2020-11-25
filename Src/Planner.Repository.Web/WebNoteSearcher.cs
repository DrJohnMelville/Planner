using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using NodaTime;
using Planner.Models.Notes;

namespace Planner.Repository.Web
{
    public class WebNoteSearcher:INoteSearcher
    {
        private readonly IJsonWebService service;

        public WebNoteSearcher(IJsonWebService service)
        {
            this.service = service;
        }

        public async IAsyncEnumerable<NoteTitle> SearchFor(string query, LocalDate minDate, LocalDate maxDate)
        {
            foreach (var item in await QueryFromWeb(query, minDate, maxDate))
            {
                yield return item;
            }
        }

        private Task<NoteTitle[]> QueryFromWeb(string query, LocalDate minDate, LocalDate maxDate) => 
            service.Get<NoteTitle[]>(
                $"/SearchNotes/{HttpUtility.UrlEncode(query)}/{minDate:yyyy-MM-dd}/{maxDate:yyyy-MM-dd}");
    }
}