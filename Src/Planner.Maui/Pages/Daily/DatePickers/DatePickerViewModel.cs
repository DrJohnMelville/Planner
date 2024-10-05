using Melville.INPC;
using NodaTime;

namespace Planner.Maui.Pages.Daily.DatePickers;

public interface IDatePickerContext
{
    void SelectDate(LocalDate date);
    void SelectOptions(DateChoicesModel neewModel);

}

public partial class DatePickerViewModel: IDatePickerContext
{
    public LocalDate Today { get; set; }
    [AutoNotify] private DateChoicesModel? choices;
    private Action<LocalDate>? selectDate;


    public void SetupDate(LocalDate date, Action<LocalDate> selectDate)
    {
        this.selectDate = selectDate;
        Choices = new MonthModel(date, this);
    }

    public void SelectDate(LocalDate date) => selectDate?.Invoke(date);

    public void SelectOptions(DateChoicesModel newChoiceModel) => 
        Choices = newChoiceModel;
}

public abstract partial class DateChoicesModel
{
    [FromConstructor] public LocalDate BaseDate { get; }
    [FromConstructor] public IDatePickerContext Actions { get; }
    public IList<DateChoiceModel> Items { get; } = new List<DateChoiceModel>();
    public abstract int Width { get; }  
    public abstract string Title { get; }
    public abstract DateChoicesModel NextBiggerStep();
    public abstract DateChoicesModel PriorStep();
    public abstract DateChoicesModel NextStep();

    private Command? upCommand = null;
    public Command UpCommand => upCommand ??= 
        new Command(() => Actions.SelectOptions(NextBiggerStep()));

    private Command? leftCommand = null;
    public Command LeftCommand => leftCommand ??=
        new Command(() => Actions.SelectOptions(NextStep()));
    private Command? rightCommand = null;
    public Command RightCommand => rightCommand ??=
        new Command(() => Actions.SelectOptions(PriorStep()));

}

public abstract partial class DateChoiceModel
{
    [FromConstructor] protected DateChoicesModel parentModel;
    [FromConstructor] protected LocalDate referenceDate { get; }
    public abstract LocalDate FirstDate { get; }
    public abstract LocalDate LastDate { get; }
    public abstract string Title { get; }

    private Command? downCommand;
    public Command DownCommand => downCommand ??=
        new Command(() => DoDownCommand());

    protected abstract void DoDownCommand();
}
