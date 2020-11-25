using System.Threading.Tasks;
using Melville.INPC;
using Melville.MVVM.AdvancedLists;
using Melville.MVVM.Wpf.DiParameterSources;
using NodaTime;
using Planner.Models.Notes;

namespace Planner.WpfViewModels.NotesSearchResults
{
    public partial class NotesSearchViewModel
    {
        public ThreadSafeBindableCollection<NoteTitle> Results { get; } = new();
        [AutoNotify] private string searchString = "";
        [AutoNotify] private LocalDate beginDate = new LocalDate(1900, 01, 01);
        [AutoNotify] private LocalDate endDate = new LocalDate(3000, 01, 01);

        public async Task DoSearch([FromServices] INoteSearcher searcher)
        {
            Results.Clear();
            await foreach (var item in searcher.SearchFor(SearchString, BeginDate, EndDate))
            {
                Results.Add(item);
            }
        }
    }
}