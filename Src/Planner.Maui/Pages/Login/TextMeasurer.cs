using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Graphics.Platform;
using Font = Microsoft.Maui.Font;

namespace Planner.Maui.Pages.Login;

public readonly struct TextMeasurer(
    PlatformStringSizeService service, 
    Microsoft.Maui.Graphics.Font font, 
    Size desiredSize)
{
    public Size Measure(string text, double fontSize) => 
        service.GetStringSize(text, font, 1f+(float)fontSize);

    public bool IsTooBig(string text, double fontSize)
    {
        var measuredSize = Measure(text, fontSize);
        return measuredSize.Width > desiredSize.Width ||
               measuredSize.Height > desiredSize.Height;
    }
    public bool IsTooWide(string text, double fontSize) => 
        Measure(text, fontSize).Width > desiredSize.Width;

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

public static class TextMeasurerFactory
{
    public static TextMeasurer CreateTecMeasurer<T>(this T item, Size desiredSize) where 
        T : IFontElement, IPaddingElement, IView, IBorderElement
    {
        return new TextMeasurer(
            new (), 
            new (item.FontFamily), 
            RemovePaddingFromSize(item, desiredSize));
    }

    private static Size RemovePaddingFromSize<T>(T item, Size desiredSize) where 
        T : IFontElement, IPaddingElement, IView, IBorderElement =>
        new(desiredSize.Width - (item.Padding.HorizontalThickness + 
                                 item.Margin.HorizontalThickness + 2*item.CornerRadius),
            desiredSize.Height - (item.Padding.VerticalThickness + 
                                  item.Margin.VerticalThickness + 2*item.CornerRadius));
}