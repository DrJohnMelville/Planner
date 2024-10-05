using NodaTime;

namespace Planner.Maui.Pages.Daily.DatePickers;

public class DecadeModel : DateChoicesModel
{
    public DecadeModel(LocalDate baseDate, IDatePickerContext context) : base(baseDate, context)
    {
        for (int i = -1; i < 11; i++)
        {
            Items.Add(new YearChoiceModel(this, new LocalDate(baseDate.Year + i, 1, 1)));
        }
    }

    public override int Width => 3;
    public override string Title => $"{BaseDate.Year}s";
    public override DateChoicesModel NextBiggerStep() => 
      new CenturyModel(new LocalDate((BaseDate.Year / 100)*100, 1,1), Actions);
    public override DateChoicesModel PriorStep() => new DecadeModel(BaseDate.PlusYears(-10), Actions);
    public override DateChoicesModel NextStep() => new DecadeModel(BaseDate.PlusYears(10), Actions);
}


public class YearChoiceModel : DateChoiceModel
{
    public YearChoiceModel(DateChoicesModel parent, LocalDate referenceDate) :
        base(parent, referenceDate)
    {
    }

    public override LocalDate FirstDate => new LocalDate(referenceDate.Year, 1,1);
    public override LocalDate LastDate => new LocalDate(referenceDate.Year, 12, 31);
    public override string Title => referenceDate.Year.ToString();
    protected override void DoDownCommand() =>
        parentModel.Actions.SelectOptions(
            new YearModel(referenceDate, parentModel.Actions));
}

public class CenturyModel: DateChoicesModel
{
    public CenturyModel(LocalDate baseDate, IDatePickerContext context) : base(baseDate, context)
    {
        for (int i = -10; i < 110; i += 10)
        {
            Items.Add(new DecadeChoiceModel(this, new LocalDate(baseDate.Year + i, 1, 1)));
        }
    }

    public override int Width => 3;
    public override string Title => $"Century {BaseDate.Year}";
    public override DateChoicesModel NextBiggerStep() => this;
    public override DateChoicesModel PriorStep() => new CenturyModel(BaseDate.PlusYears(-100), Actions);
    public override DateChoicesModel NextStep() => new CenturyModel(BaseDate.PlusYears(100), Actions);
}

public class DecadeChoiceModel : DateChoiceModel
{
    public DecadeChoiceModel(DateChoicesModel parent, LocalDate referenceDate) :
        base(parent, referenceDate)
    {
    }

    public override LocalDate FirstDate => new LocalDate(referenceDate.Year, 1, 1);
    public override LocalDate LastDate => new LocalDate(referenceDate.Year + 9, 12, 31);
    public override string Title => $"{referenceDate.Year}s";
    protected override void DoDownCommand() =>
        parentModel.Actions.SelectOptions(
            new DecadeModel(referenceDate, parentModel.Actions));
}