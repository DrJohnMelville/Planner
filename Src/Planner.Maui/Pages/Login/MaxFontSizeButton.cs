using Microsoft.Maui.Graphics.Platform;

namespace Planner.Maui.Pages.Login;

public class MaxFontSizeButton: Button
{
    #warning use IStringSizeService
//    private readonly IStringSizeService sizeService = new PlatformStringSizeService();

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
        constraint = new Size(
            widthConstraint, heightConstraint);

        FontSize = (IsTooBig(Text, FontSize) ?
            MaxSize(2, FontSize, Text) :
            MaxSize(FontSize, 1000, Text)) * 0.75;
        InvalidateMeasure();
    }

    public bool IsTooBig(string text, double fontSize)
    {
        FontSize = fontSize;
        var measuredSize = base.MeasureOverride(constraint.Width, constraint.Height);
        return measuredSize.Width > constraint.Width ||
               measuredSize.Height > constraint.Height;
    }
    public double MaxSize(double bottom, double top, string text)
    {
        while (top - bottom > 1.0)
        {
            var mean = (top + bottom) / 2.0;
            if (IsTooBig(text, mean))
                top = mean;
            else
                bottom = mean;
        }
        return bottom;
    }
}