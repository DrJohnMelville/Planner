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
        private readonly Func<LocalDate, DailyNotesViewModel> notesFactory;

        [AutoNotify] private LocalDate currentDate;
        [AutoNotify] private DailyTaskListViewModel todayTaskList;
        [AutoNotify] private DailyNotesViewModel todayNoteList;
        [AutoNotify] private bool popupOpen;
        
        partial void WhenCurrentDateChanges(LocalDate oldValue, LocalDate newValue)
        {
            TodayTaskList = taskListFactory(newValue);
            TodayNoteList = notesFactory(newValue);
        }

        public DailyPlannerPageViewModel(
            IClock clock, 
            Func<LocalDate, DailyTaskListViewModel> taskListFactory, 
            Func<LocalDate, DailyNotesViewModel> notesFactory)
        {
            this.clock = clock;
            this.taskListFactory = taskListFactory;
            this.notesFactory = notesFactory;
            currentDate = clock.CurrentDate();
            todayTaskList = taskListFactory(currentDate);
            todayNoteList = notesFactory(currentDate);
        }

        public void ForwardOneDay() => CurrentDate = CurrentDate.PlusDays(1);
        public void BackOneDay() => CurrentDate = CurrentDate.PlusDays(-1);
    }
}