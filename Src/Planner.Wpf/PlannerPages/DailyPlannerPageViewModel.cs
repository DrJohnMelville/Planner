using System;
using System.Collections.Generic;
using Melville.INPC;
using Melville.MVVM.Wpf.DiParameterSources;
using Melville.MVVM.Wpf.EventBindings;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Repositories;
using Planner.Models.Time;
using Planner.Wpf.Appointments;
using Planner.Wpf.Notes;
using DailyTaskListViewModel = Planner.Wpf.TaskList.DailyTaskListViewModel;

namespace Planner.Wpf.PlannerPages
{
    public interface ILinkRedirect
    {
        bool? DoRedirect(string url);
    }

    public partial class DailyPlannerPageViewModel:PageWithEditNotifications, IAdditionlTargets
    {
        private readonly IPlannerNavigator navigator;
        private readonly Func<LocalDate, RichTextCommandTarget> richCommandFactory;
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
            ILinkRedirect redirect,
            Func<LocalDate,RichTextCommandTarget> richCommandFactory): base(noteEditRequest, redirect)
        {
            this.navigator = navigator;
            this.richCommandFactory = richCommandFactory;
            this.currentDate = currentDate; 
            TodayTaskList = taskListFactory(currentDate);
            JournalPage = noteDisplayFactory(currentDate);
            Appointments = appointmentFactory(currentDate);
        }

        public void ForwardOneDay() => navigator.ToDate(CurrentDate.PlusDays(1));
        public void BackOneDay() => navigator.ToDate(CurrentDate.PlusDays(-1));

        protected override void DoEditNoteRequest(object? sender, NoteEditRequestEventArgs e) =>
            navigator.ToEditNote(e);


        public void ReloadCaches([FromServices]IEventBroadcast<ClearCachesEventArgs> signalObject) => 
            signalObject.Fire(this, new ClearCachesEventArgs());

        public void GoToToday([FromServices] IUsersClock clock) => navigator.ToDate(clock.CurrentDate());
        public void SearchJournal() => navigator.ToNoteSearchPage();
        
        IEnumerable<object> IAdditionlTargets.Targets() => new []{richCommandFactory(currentDate)};
    }

}