using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Melville.INPC;
using Melville.Lists;
using Melville.MVVM.WaitingServices;
using Melville.MVVM.Wpf.DiParameterSources;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Notes;
using Planner.Wpf.PlannerPages;

namespace Planner.Wpf.NotesSearchResults
{
    public partial class NotesSearchViewModel: PageWithEditNotifications
    {
        private readonly INoteUrlGenerator urlGen;
        private readonly IPlannerNavigator navigator;
        public NotesSearchViewModel(INoteUrlGenerator urlGen,
            IEventBroadcast<NoteEditRequestEventArgs> notifier, 
            IPlannerNavigator navigator,
            ILinkRedirect redirect) : base(notifier, redirect)
        {
            this.urlGen = urlGen;
            this.navigator = navigator;
            displayUrl = urlGen.ArbitraryNoteView(Array.Empty<Guid>());
        }

        public ThreadSafeBindableCollection<NoteTitle> Results { get; } = new();
        [AutoNotify] private string displayUrl;
        [AutoNotify] private string searchString = "";
        [AutoNotify] private LocalDate beginDate = new LocalDate(1900, 01, 01);
        [AutoNotify] private LocalDate endDate = new LocalDate(3000, 01, 01);

        public async Task DoSearch([FromServices] INoteSearcher searcher,
             IWaitingService waiter)
        {
            using var _ = waiter.WaitBlock("Searching");
            Results.Clear();
            await foreach (var item in searcher.SearchFor(SearchString, BeginDate, EndDate))
            {
                Results.Add(item);
            }
        }

        public void NewItemsSelected(IList selected) => 
            DisplayUrl = urlGen.ArbitraryNoteView(selected.OfType<NoteTitle>().Select(i=>i.Key));

        protected override void DoEditNoteRequest(object? sender, NoteEditRequestEventArgs e) =>
            navigator.ToDate(e.Note.Date);
    }
}