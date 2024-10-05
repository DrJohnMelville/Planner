namespace Planner.Maui.Pages.Login;

public class MaxFontSizeButton: Button
{
    public MaxFontSizeButton()
    {
        LineBreakMode = LineBreakMode.NoWrap;
    }

    private Size constraint = Size.Zero;

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        if (constraint.Width != widthConstraint || constraint.Height != heightConstraint) 
            FindMaxFontSize(widthConstraint, heightConstraint);
 
        return base.MeasureOverride(widthConstraint, heightConstraint);
    }
     
    private void FindMaxFontSize(double widthConstraint, double heightConstraint)
    {
        var twoCorners = 2.0 * CornerRadius;
        constraint = new Size(
            widthConstraint, heightConstraint);
        var measure = this.CreateTecMeasurer(constraint);
        FontSize = (measure.IsTooBig(Text, FontSize) ?
            measure.MaxSize(2, FontSize, Text) :
            measure.MaxSize(FontSize, 1000, Text)) * 0.75;
        InvalidateMeasure();
    }
}