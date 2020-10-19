using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using Melville.INPC;
using Melville.MVVM.Wpf.RootWindows;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Tasks;
using Planner.WpfViewModels.Notes.Pasters;
using Planner.WpfViewModels.TaskList;

namespace Planner.WpfViewModels.PlannerPages
{
    public partial class DailyPlannerPageViewModel:IAcceptNavigationNotifications
    {
        private readonly INotesServer noteServer;
        private readonly IPlannerNavigator navigator;
        private readonly INoteUrlGenerator urlGen;
        private readonly IMarkdownPaster paster;
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
            INotesServer noteServer,
            NoteCreator noteCreator, 
            IPlannerNavigator navigator, 
            INoteUrlGenerator urlGen, 
            IMarkdownPaster paster)
        {
            this.noteServer = noteServer;
            this.navigator = navigator;
            NoteCreator = noteCreator;
            this.urlGen = urlGen;
            this.paster = paster;
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
        
        public void NavigatedTo() => noteServer.NoteEditRequested += DoEditNoteRequest;
        public void NavigatedAwayFrom() => noteServer.NoteEditRequested -= DoEditNoteRequest;

        private void DoEditNoteRequest(object? sender, NoteEditRequestEventArgs e) =>
            navigator.ToEditNote(e);

        public void PlannerPageLinkClicked(Segment<TaskTextType> segment)
        {
            if (segment.Match == null) return;
            navigator.NavigateToDate(segment.Match.Groups, CurrentDate);
        }
    }
}