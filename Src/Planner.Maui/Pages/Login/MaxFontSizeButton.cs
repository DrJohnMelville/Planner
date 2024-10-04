using System.Drawing.Drawing2D;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

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
        if (constraint.Width != widthConstraint || constraint.Height != heightConstraint) FindMaxFontSize(widthConstraint, heightConstraint);

        return new Size(widthConstraint, heightConstraint);
    }

    private void FindMaxFontSize(double widthConstraint, double heightConstraint)
    {
        constraint = new Size(widthConstraint, heightConstraint);
        var startingSize = FontSize;
        if (IsTooBig())
            SearchSize(2, startingSize);
        else 
            SearchSize(startingSize, 1000);
    }


    private bool IsTooBig()
    {
         var size = base.MeasureOverride(1_000_000, 1_000_000);
         return (size.Width > constraint.Width || size.Height > constraint.Height);
    }

    private void SearchSize(double bottom, double top)
    {
        while (top - bottom > 1.0)
        {
            var mean = (top + bottom) / 2.0;
            FontSize = mean;
            if (IsTooBig())
                top = mean;
            else
                bottom = mean;
        }
        FontSize = bottom;
    }
}