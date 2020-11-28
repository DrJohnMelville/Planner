using System;
using Melville.INPC;
using Melville.MVVM.Wpf.DiParameterSources;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Repositories;
using Planner.Models.Tasks;
using Planner.Models.Time;
using Planner.WpfViewModels.TaskList;

namespace Planner.WpfViewModels.PlannerPages
{
    public interface ILinkRedirect
    {
        bool? DoRedirect(string url);
    }
    
    public partial class DailyPlannerPageViewModel:PageWithEditNotifications
    {
        private readonly IPlannerNavigator navigator;
        private readonly INoteUrlGenerator urlGen;
        public NoteCreator NoteCreator { get; }
        public DailyTaskListViewModel TodayTaskList { get; }
        private readonly LocalDate currentDate;


        public LocalDate CurrentDate
        {
            get => currentDate;
            set => navigator.ToDate(value);
        }

        [AutoNotify] private bool popupOpen;
        public string NotesUrl => urlGen.DailyUrl(CurrentDate);

            public DailyPlannerPageViewModel(
            LocalDate currentDate,
            Func<LocalDate, DailyTaskListViewModel> taskListFactory, 
            INotesServer noteServer, // we don't use this, we just need it to exist.  Asking for it forces it to exist.
            NoteCreator noteCreator, 
            IPlannerNavigator navigator, 
            INoteUrlGenerator urlGen, 
            IEventBroadcast<NoteEditRequestEventArgs> noteEditRequest, 
            ILinkRedirect redirect): base(noteEditRequest, redirect)
        {
            
            this.navigator = navigator;
            NoteCreator = noteCreator;
            this.urlGen = urlGen;
            this.currentDate = currentDate; 
            TodayTaskList = taskListFactory(currentDate);
        }

        public void ForwardOneDay() => navigator.ToDate(CurrentDate.PlusDays(1));
        public void BackOneDay() => navigator.ToDate(CurrentDate.PlusDays(-1));

        public void CreateNoteOnDay()
        {
            NoteCreator.Create(CurrentDate);
            ReloadNotesDisplay();
        }
        //The notes url includes a nonce so we can force updates when the data changes.
        // all we have to do is tell wpf that the url changed and it will read a new
        // value and refresh the webbrowser.
        private void ReloadNotesDisplay() => 
            ((IExternalNotifyPropertyChanged) this).OnPropertyChanged(nameof(NotesUrl));

        protected override void DoEditNoteRequest(object? sender, NoteEditRequestEventArgs e) =>
            navigator.ToEditNote(e);

        public void PlannerPageLinkClicked(Segment<TaskTextType> segment)
        {
            if (segment.Match == null) return;
            navigator.NavigateToDate(segment.Match.Groups, CurrentDate);
        }

        public void ReloadCaches([FromServices]IEventBroadcast<ClearCachesEventArgs> signalObject)
        {
            signalObject.Fire(this, new ClearCachesEventArgs());
        }

        public void GoToToday([FromServices] IClock clock)
        {
            navigator.ToDate(clock.CurrentDate());
        }

        public void SearchJournal() => navigator.ToNoteSearchPage();
    }

}