using NodaTime;

namespace Planner.Maui.Pages.Daily.DatePickers;

public class YearModel : DateChoicesModel
{
    public YearModel(LocalDate baseDate, IDatePickerContext context) : base(baseDate, context)
    {
        for (int i = 1; i < 13; i++)
        {
            Items.Add(new MonthChoiceModel(this, new LocalDate(baseDate.Year, i, 1)));
        }
    }

    public override int Width => 3;
    public override string Title => BaseDate.Year.ToString();
    public override DateChoicesModel NextBiggerStep() => 
        new DecadeModel(new LocalDate((BaseDate.Year/10)*10, 1,1), Actions);
    public override DateChoicesModel PriorStep() => new YearModel(BaseDate.PlusYears(-1), Actions);
    public override DateChoicesModel NextStep() => new YearModel(BaseDate.PlusYears(1), Actions);
}


public class MonthChoiceModel(DateChoicesModel parent, LocalDate baseDate) : 
    DateChoiceModel(parent, baseDate)
{
    public override LocalDate FirstDate => referenceDate.With(DateAdjusters.StartOfMonth);
    public override LocalDate LastDate => referenceDate.With(DateAdjusters.EndOfMonth);
    public override string Title => referenceDate.ToString("MMM", null);
    protected override void DoDownCommand() =>
        parentModel.Actions.SelectOptions(new MonthModel(referenceDate, parentModel.Actions));
}
