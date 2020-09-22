using System;
using System.Diagnostics;
using Melville.INPC;
using NodaTime;
using Planner.Models.Time;
using Planner.WpfViewModels.TaskList;

namespace Planner.WpfViewModels.PlannerPages
{
    public partial class DailyPlannerPageViewModel
    {
        private readonly IClock clock;
        private readonly Func<LocalDate, DailyTaskListViewModel> taskListFactory;

        [AutoNotify] private LocalDate currentDate;
        [AutoNotify] private DailyTaskListViewModel todayTaskList;

        public DailyPlannerPageViewModel(IClock clock, Func<LocalDate, DailyTaskListViewModel> taskListFactory)
        {
            this.clock = clock;
            this.taskListFactory = taskListFactory;
            currentDate = clock.CurrentDate();
            todayTaskList = taskListFactory(currentDate);
        }
    }
}