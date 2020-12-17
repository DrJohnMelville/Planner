using System;
using Melville.INPC;
using Melville.MVVM.RunShellCommands;
using Melville.MVVM.Wpf.DiParameterSources;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Repositories;
using Planner.Models.Tasks;
using Planner.Models.Time;
using Planner.WpfViewModels.Appointments;
using Planner.WpfViewModels.Notes;
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
        public DailyTaskListViewModel TodayTaskList { get; }
        public DailyNoteDisplayViewModel JournalPage { get; }
        public DailyAppointmentsViewModel Appointments { get; }
        [AutoNotify] private bool popupOpen;
        
        private readonly LocalDate currentDate;
        public LocalDate CurrentDate
        {
            get => currentDate;
            set => navigator.ToDate(value);
        }

        public DailyPlannerPageViewModel(
            LocalDate currentDate,
            Func<LocalDate, DailyTaskListViewModel> taskListFactory,
            Func<LocalDate, DailyNoteDisplayViewModel> noteDisplayFactory,
            Func<LocalDate, DailyAppointmentsViewModel> appointmentFactory,
            IPlannerNavigator navigator, 
            IEventBroadcast<NoteEditRequestEventArgs> noteEditRequest, 
            ILinkRedirect redirect): base(noteEditRequest, redirect)
        {
            this.navigator = navigator;
            this.currentDate = currentDate; 
            TodayTaskList = taskListFactory(currentDate);
            JournalPage = noteDisplayFactory(currentDate);
            Appointments = appointmentFactory(currentDate);
        }

        public void ForwardOneDay() => navigator.ToDate(CurrentDate.PlusDays(1));
        public void BackOneDay() => navigator.ToDate(CurrentDate.PlusDays(-1));

        protected override void DoEditNoteRequest(object? sender, NoteEditRequestEventArgs e) =>
            navigator.ToEditNote(e);

        public void PlannerPageLinkClicked(Segment<TaskTextType> segment)
        {
            if (segment.Match == null) return;
            navigator.NavigateToDate(segment.Match.Groups, CurrentDate);
        }

        public void ReloadCaches([FromServices]IEventBroadcast<ClearCachesEventArgs> signalObject) => 
            signalObject.Fire(this, new ClearCachesEventArgs());

        public void GoToToday([FromServices] IUsersClock clock) => navigator.ToDate(clock.CurrentDate());
        public void SearchJournal() => navigator.ToNoteSearchPage();
        
        
        public void WebLinkLinkClicked(
            Segment<TaskTextType> segment, 
            [FromServices] IRunShellCommand commandObject) =>
            commandObject.ShellExecute(segment.Text, Array.Empty<string>());
        public void FileLinkLinkClicked(
            Segment<TaskTextType> segment, 
            [FromServices] IRunShellCommand commandObject) =>
            commandObject.ShellExecute(segment.Text, Array.Empty<string>());


    }

}