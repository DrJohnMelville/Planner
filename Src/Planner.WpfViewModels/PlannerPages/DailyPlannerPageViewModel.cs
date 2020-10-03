using System;
using Melville.INPC;
using NodaTime;
using Planner.Models.Time;
using Planner.WpfViewModels.Notes;
using Planner.WpfViewModels.TaskList;

namespace Planner.WpfViewModels.PlannerPages
{
    public partial class DailyPlannerPageViewModel
    {
        private readonly IClock clock;
        private readonly Func<LocalDate, DailyTaskListViewModel> taskListFactory;
        private readonly INotesServer noteServer;

        [AutoNotify] private LocalDate currentDate;
        [AutoNotify] private DailyTaskListViewModel todayTaskList;
        [AutoNotify] private bool popupOpen;
        public string NotesUrl => noteServer.BaseUrl+currentDate.ToString("yyyy-M-d", null);

            partial void WhenCurrentDateChanges(LocalDate oldValue, LocalDate newValue)
        {
            TodayTaskList = taskListFactory(newValue);
            ((IExternalNotifyPropertyChanged)this).OnPropertyChanged(nameof(NotesUrl));
        }

        public DailyPlannerPageViewModel(
            IClock clock, 
            Func<LocalDate, DailyTaskListViewModel> taskListFactory, INotesServer noteServer)
        {
            this.clock = clock;
            this.taskListFactory = taskListFactory;
            this.noteServer = noteServer;
            currentDate = clock.CurrentDate();
            todayTaskList = taskListFactory(currentDate);
        }

        public void ForwardOneDay() => CurrentDate = CurrentDate.PlusDays(1);
        public void BackOneDay() => CurrentDate = CurrentDate.PlusDays(-1);
    }
}