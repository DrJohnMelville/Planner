using Melville.INPC;
using Microsoft.Maui.Layouts;

namespace Planner.Maui.Pages.Login;

[GenerateBP(typeof(int), "MaxColumns", Default = int.MaxValue)]
[GenerateBP(typeof(int), "MaxRows", Default = int.MaxValue)]
public partial class FillItemsLayout : Layout
{
    protected override ILayoutManager CreateLayoutManager() => 
        new FillItemsLayoutManager(this);
}

public class FillItemsLayoutManager(FillItemsLayout layout) : ILayoutManager
{
    private (int columns, int rows) GridDimensions() =>
        (layout.MaxColumns, layout.MaxRows) switch
        {
            (var cols, int.MaxValue) => (cols, EnoughGroupsForItems(cols)),
            (int.MaxValue, var rows) => (EnoughGroupsForItems(rows), rows),
            var x => x
        };

    private int EnoughGroupsForItems(int groupSize) => 
        (layout.Count + groupSize - 1) / groupSize;

    public Size ArrangeChildren(Rect bounds)
    {
        var (cols, rows) = GridDimensions();
        var cellSize = ComputeCellSize(bounds, cols, rows);
        var location = new Rect(new Point(
            layout.Padding.Left, layout.Padding.Top), cellSize);
        int item = 0;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (item >= layout.Count) return bounds.Size;
                var view = layout[item++];
                view.Measure(location.Width, location.Height);
                view.Arrange(location);
                location.X += cellSize.Width;
            }
            location.X = layout.Padding.Left;
            location.Y += cellSize.Height;
        }
        return bounds.Size;
    }

    private Size ComputeCellSize(Rect bounds, int cols, int rows)
    {
        return new Size(
            (bounds.Width - layout.Padding.HorizontalThickness)  / cols, 
            (bounds.Height- layout.Padding.VerticalThickness) / rows);
    }

    public Size Measure(double widthConstraint, double heightConstraint) => 
        new(widthConstraint, heightConstraint);
}