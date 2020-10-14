using System;
using System.IO;
using System.Text.RegularExpressions;
using Melville.INPC;
using Melville.MVVM.Wpf.RootWindows;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Tasks;
using Planner.Models.Time;
using Planner.WpfViewModels.Notes;
using Planner.WpfViewModels.TaskList;

namespace Planner.WpfViewModels.PlannerPages
{
    public partial class DailyPlannerPageViewModel:IAcceptNavigationNotifications
    {
        private readonly Func<LocalDate, DailyTaskListViewModel> taskListFactory;
        private readonly INotesServer noteServer;
        private readonly INavigationWindow navigation;
        private Func<NoteEditRequestEventArgs, NoteEditorViewModel> editorFactory;
        private readonly INoteUrlGenerator urlGen;
        public NoteCreator NoteCreator { get; }

        [AutoNotify] private LocalDate currentDate;
        [AutoNotify] private DailyTaskListViewModel todayTaskList;
        [AutoNotify] private bool popupOpen;
        [AutoNotify] public string NotesUrl => urlGen.DailyUrl(CurrentDate);
        partial void WhenCurrentDateChanges(LocalDate oldValue, LocalDate newValue)
            {
                TodayTaskList = taskListFactory(newValue);
            }

            private void RefreshNotesUrl() => 
                ((IExternalNotifyPropertyChanged) this).OnPropertyChanged(nameof(NotesUrl));

            public DailyPlannerPageViewModel(
            IClock clock, 
            Func<LocalDate, DailyTaskListViewModel> taskListFactory, 
            INotesServer noteServer,
            NoteCreator noteCreator, 
            INavigationWindow navigation, 
            Func<NoteEditRequestEventArgs, NoteEditorViewModel> editorFactory, INoteUrlGenerator urlGen)
        {
            this.taskListFactory = taskListFactory;
            this.noteServer = noteServer;
            NoteCreator = noteCreator;
            this.navigation = navigation;
            this.editorFactory = editorFactory;
            this.urlGen = urlGen;
            currentDate = clock.CurrentDate();
            todayTaskList = taskListFactory(currentDate);
        }

        public void ForwardOneDay() => CurrentDate = CurrentDate.PlusDays(1);
        public void BackOneDay() => CurrentDate = CurrentDate.PlusDays(-1);

        public void CreateNoteOnDay()
        {
            NoteCreator.Create(CurrentDate);
            RefreshNotesUrl();
        }

        public void NavigatedTo() => noteServer.NoteEditRequested += LaunchRequest;
        public void NavigatedAwayFrom() => noteServer.NoteEditRequested -= LaunchRequest;

        private void LaunchRequest(object? sender, NoteEditRequestEventArgs e) => 
            navigation.NavigateTo(editorFactory(e));

        public void PlannerPageLinkClicked(Segment<TaskTextType> segment) =>
            CurrentDate = new LocalDate(GetLinkYear(segment.Match.Groups),
                GetSegmentValue(segment, 1), GetSegmentValue(segment, 2));

        private int GetLinkYear(GroupCollection matchGroups) => 
            matchGroups.Count == 5 ? GetLinkYear(matchGroups[3].Value): currentDate.Year;

        private int GetLinkYear(string yearString)
        {
            var rawYearIndicator = int.Parse(yearString);
            return rawYearIndicator < 100 ? rawYearIndicator + CurrentCentury(CurrentDate) : rawYearIndicator;
        }

        private int CurrentCentury(LocalDate date) => date.Year - (date.Year % 100);

        private static int GetSegmentValue(Segment<TaskTextType> segment, int index) => 
            int.Parse(segment.Match.Groups[index].Value);
    }
}