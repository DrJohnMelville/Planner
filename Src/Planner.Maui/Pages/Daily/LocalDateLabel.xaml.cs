using Melville.INPC;
using Microsoft.Maui.Layouts;
using NodaTime;

namespace Planner.Maui.Pages.Daily;

[GenerateBP(typeof(double), "FontSize")]
[GenerateBP(typeof(string), "FontFamily")]
public partial class LocalDateLabel : ContentView
{
    [GenerateBP]
    private void OnDateChanged(LocalDate date) =>
        InvalidateMeasure();

	public LocalDateLabel()
    {
        InitializeComponent();
	}
}

[GenerateBP(typeof(LocalDate), "Date")]
public partial class AdaptiveTextLayout: Layout
{
    protected override ILayoutManager CreateLayoutManager() => 
        new AdaptiveTextLayoutManager(this);
}

public class AdaptiveTextLayoutManager(AdaptiveTextLayout layout) : ILayoutManager
{
    private static String[] FormatOptions = [
        "D",
        "dddd MMM d, yyyy",
        "ddd MMM d, yyyy",
        "ddd MM/d/yyyy",
        "d"
    ];

    private Label GetChild()
    {
        if (layout.Count != 1)
            throw new InvalidOperationException(
                "AdaptiveTextLayout must have exactly one child.");
        return (Label)layout[0];
    }

    public Size Measure(double widthConstraint, double heightConstraint)
    {
        var child = GetChild();
        Size size = default;
        foreach (var format in FormatOptions)
        {
            child.Text = layout.Date.ToString(format, null);
            size = child.Measure(double.PositiveInfinity, heightConstraint).Request;
            if (size.Width <= widthConstraint) break;
        }
        return child.Measure(widthConstraint, heightConstraint);
    }

    public Size ArrangeChildren(Rect bounds)
    {
        layout[0].Arrange(bounds);
        return bounds.Size;
    }
}
