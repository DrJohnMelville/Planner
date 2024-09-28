using Melville.Lists.PersistentLinq;
using NodaTime;
using Planner.Models.Repositories;
using Planner.Models.Tasks;
using Planner.Models.Time;

namespace Planner.Maui.Pages.Daily;

public class DailyPageViewModel
{
    public LocalDate Date { get; set; }
    public TaskViewModel Tasks { get; }
    public string Text => $"Daily Page View: {Date}";
    public DailyPageViewModel(IUsersClock clock, Func<LocalDate, TaskViewModel> taskFactory)
    {
        Date = clock.CurrentDate();
        Tasks = taskFactory(Date);
    }

}

public class TaskViewModel(
    ILocalRepository<PlannerTask> taskRepository, LocalDate date)
{
    public IList<SingleTaskViewModel> Tasks { get; } =
        taskRepository.ItemsForDate(date)
            .SelectCol(i => new SingleTaskViewModel(i));
}

public class SingleTaskViewModel(PlannerTask task)
{
    public PlannerTask Task { get; set; } = task;

}