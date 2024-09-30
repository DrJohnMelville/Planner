using Melville.INPC;
using Melville.MVVM.Maui.Commands;
using NodaTime;
using Planner.Maui.Pages.Daily.Tasks;
using Planner.Models.Time;

namespace Planner.Maui.Pages.Daily;

public partial class DailyPageViewModel
{
    [AutoNotify] private LocalDate date;
    [AutoNotify] private TaskViewModel tasks;
    private readonly Func<LocalDate, TaskViewModel> taskFactory;
    private readonly IUsersClock clock;

    public Command TomorrowCommand { get; }
    public Command YesterdayCommand { get; }
    public Command TodayCommand { get; }
    public CommandBase PickDateCommand { get; }

    public DailyPageViewModel(IUsersClock clock, Func<LocalDate, TaskViewModel> taskFactory)
    {
        this.taskFactory = taskFactory;
        this.clock = clock;
        TomorrowCommand = new Command(() => Date = Date.PlusDays(1));
        YesterdayCommand = new Command(() => Date = Date.PlusDays(-1));
        TodayCommand = new Command(() => Date = this.clock.CurrentDate(), 
            ()=> Date != this.clock.CurrentDate());
        PickDateCommand = InheritedCommandFactory.Create(PickDate);
        Date = clock.CurrentDate();
    }

    private void OnDateChanged(LocalDate date)
    {
        Tasks = taskFactory(date);
        TodayCommand.ChangeCanExecute();
    }

    public Task PickDate(INavigation navigation, 
        [FromServices] DatePickerPage pickerPage )
    {
        return navigation.PushModalAsync(pickerPage);
    }
}
