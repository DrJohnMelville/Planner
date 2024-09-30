using Melville.Lists.PersistentLinq;
using NodaTime;
using Planner.Models.Repositories;
using Planner.Models.Tasks;

namespace Planner.Maui.Pages.Daily.Tasks;


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