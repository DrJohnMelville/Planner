using Melville.INPC;
using NodaTime;

namespace Planner.Maui.Pages.Daily;

public partial class LocalDateLabel : Label
{
    private static String[] FormatOptions = [
        "D",
        "dddd MMM d, yyyy",
        "ddd MMM d, yyyy",
        "ddd MM/d/yyyy",
        "d"
    ];

    [GenerateBP]
    private void OnDateChanged(LocalDate date) => InvalidateMeasure();

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        foreach (var format in FormatOptions)
        {
            Text = Date.ToString(format, null);
            var size = base.MeasureOverride(double.PositiveInfinity, heightConstraint);
            if (size.Width <= widthConstraint) break;
        }
        return base.MeasureOverride(widthConstraint, heightConstraint);
    }
}