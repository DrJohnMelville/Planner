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
        private readonly INotesServer noteServer;
        private readonly IPlannerNavigator navigator;
        private readonly INoteUrlGenerator urlGen;
        public NoteCreator NoteCreator { get; }

        private readonly LocalDate currentDate;
        public LocalDate CurrentDate
        {
            get => currentDate;
            set => navigator.ToDate(value);
        }

        public DailyTaskListViewModel TodayTaskList { get; }
        [AutoNotify] private bool popupOpen;
        public string NotesUrl => urlGen.DailyUrl(CurrentDate);

            private void RefreshNotesUrl() => 
                ((IExternalNotifyPropertyChanged) this).OnPropertyChanged(nameof(NotesUrl));

            public DailyPlannerPageViewModel(
            LocalDate currentDate,
            Func<LocalDate, DailyTaskListViewModel> taskListFactory, 
            INotesServer noteServer,
            NoteCreator noteCreator, 
            IPlannerNavigator navigator, INoteUrlGenerator urlGen)
        {
            this.noteServer = noteServer;
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
            RefreshNotesUrl();
        }

        public void NavigatedTo() => noteServer.NoteEditRequested += LaunchRequest;
        public void NavigatedAwayFrom() => noteServer.NoteEditRequested -= LaunchRequest;

        private void LaunchRequest(object? sender, NoteEditRequestEventArgs e) =>
            navigator.ToEditNote(e);

        public void PlannerPageLinkClicked(Segment<TaskTextType> segment)
        {
            if (segment.Match == null) return;
            navigator.ToDate(new LocalDate(GetLinkYear(segment.Match.Groups),
                GetSegmentValue(1, segment.Match), GetSegmentValue(2, segment.Match)));
        }

        private int GetLinkYear(GroupCollection matchGroups) => 
            matchGroups.Count == 5 ? GetLinkYear(matchGroups[3].Value): CurrentDate.Year;

        private int GetLinkYear(string yearString)
        {
            var rawYearIndicator = int.Parse(yearString);
            return rawYearIndicator < 100 ? rawYearIndicator + CurrentCentury(CurrentDate) : rawYearIndicator;
        }

        private int CurrentCentury(LocalDate date) => date.Year - (date.Year % 100);

        private static int GetSegmentValue(int index, Match match) => 
            int.Parse(match.Groups[index].Value);
    }
}