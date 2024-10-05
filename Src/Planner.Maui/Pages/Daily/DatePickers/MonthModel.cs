using NodaTime;

namespace Planner.Maui.Pages.Daily.DatePickers;

public class MonthModel : DateChoicesModel
{
    public MonthModel(LocalDate baseDate, IDatePickerContext actions) : base(baseDate, actions)
    {
        var first = baseDate
            .With(DateAdjusters.StartOfMonth)
            .With(DateAdjusters.PreviousOrSame(IsoDayOfWeek.Sunday));
        var last = baseDate.With(DateAdjusters.EndOfMonth)
            .With(DateAdjusters.NextOrSame(IsoDayOfWeek.Saturday));
        for (var date = first; date <= last; date = date.PlusDays(1))
        {
            Items.Add(new DayChoiceModel(this, date));
        }
    }

    public override int Width => 7;
    public override string Title => BaseDate.ToString("MMM yyyy", null);
    public override DateChoicesModel NextBiggerStep() => 
        new YearModel(new LocalDate(BaseDate.Year, 1, 1), Actions);

    public override DateChoicesModel PriorStep() => new MonthModel(BaseDate.PlusMonths(-1), Actions);
    public override DateChoicesModel NextStep() => new MonthModel(BaseDate.PlusMonths(1), Actions);
}

public class DayChoiceModel(DateChoicesModel parentModel, LocalDate referenceDate)
    : DateChoiceModel(parentModel, referenceDate)
{
    public override LocalDate FirstDate => referenceDate;
    public override LocalDate LastDate => referenceDate;
    public override string Title => referenceDate.Day.ToString();

    protected override void DoDownCommand() => parentModel.Actions.SelectDate(referenceDate);
}
