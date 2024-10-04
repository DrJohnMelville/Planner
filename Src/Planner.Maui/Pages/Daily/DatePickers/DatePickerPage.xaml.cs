using Melville.INPC;
using NodaTime;

namespace Planner.Maui.Pages.Daily.DatePickers;

public partial class DatePickerPage : ContentPage
{
	public DatePickerViewModel ViewModel { get; } = new DatePickerViewModel();
    public DatePickerPage()
	{
		InitializeComponent();
        BindingContext = ViewModel;
    }
}

public partial class DatePickerViewModel
{
    public LocalDate Today { get; set; }
    [AutoNotify] private DateChoicesModel choices = NullDateChoicesModel.Instance;

    public void SetupDate(LocalDate date)
    {
        Choices = new MonthModel(date);
    }
}

public abstract partial class DateChoicesModel
{
    [FromConstructor] public LocalDate BaseDate { get; }
    public IList<DateChoiceModel> Items { get; } = new List<DateChoiceModel>();
    public abstract int Width { get; }
    public abstract string Title { get; }
    public abstract DateChoicesModel NextBiggerStep();
}

public partial class NullDateChoicesModel() : DateChoicesModel(default)
{
    public static readonly NullDateChoicesModel Instance = new();
    public override int Width => 1;

    public override string Title => "Error";

    public override DateChoicesModel NextBiggerStep() => this;
}

public abstract partial class DateChoiceModel
{
    [FromConstructor] protected DateChoicesModel parentModel;
    [FromConstructor] protected LocalDate referenceDate { get; }
    public abstract LocalDate FirstDate { get; }
    public abstract LocalDate LastDate { get; }
    public abstract string Title { get; }
}

