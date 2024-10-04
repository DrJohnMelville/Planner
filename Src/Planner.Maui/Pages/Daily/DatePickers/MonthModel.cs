using NodaTime;

namespace Planner.Maui.Pages.Daily.DatePickers;

public class MonthModel : DateChoicesModel
{
    public MonthModel(LocalDate baseDate) : base(baseDate)
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
        new YearModel(new LocalDate(BaseDate.Year, 1, 1));
}

public class DayChoiceModel(DateChoicesModel parentModel, LocalDate referenceDate)
    : DateChoiceModel(parentModel, referenceDate)
{
    public override LocalDate FirstDate => referenceDate;
    public override LocalDate LastDate => referenceDate;
    public override string Title => referenceDate.Day.ToString();
}

public class YearModel : DateChoicesModel
{
    public YearModel(LocalDate baseDate) : base(baseDate)
    {
        for (int i = 1; i < 13; i++)
        {
            Items.Add(new MonthChoiceModel(this, new LocalDate(baseDate.Year, i, 1)));
        }
    }

    public override int Width => 3;
    public override string Title => BaseDate.Year.ToString();
    public override DateChoicesModel NextBiggerStep() => this;
}

public class MonthChoiceModel(DateChoicesModel parent, LocalDate baseDate) : 
    DateChoiceModel(parent, baseDate)
{
    public override LocalDate FirstDate => referenceDate.With(DateAdjusters.StartOfMonth);
    public override LocalDate LastDate => referenceDate.With(DateAdjusters.EndOfMonth);
    public override string Title => referenceDate.ToString("MMM", null);
}