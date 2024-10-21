using Melville.INPC;

namespace Planner.Maui.Pages.Daily.Tasks;

[GenerateBP(typeof(Color), "Color")]
public partial class ColumnLines: BindableObject, IDrawable
{
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        
        canvas.StrokeColor = Color;
        canvas.StrokeSize = 1;
        canvas.DrawLine(30, 0, 30, dirtyRect.Height);
        canvas.DrawLine(70, 0, 70, dirtyRect.Height);
        canvas.DrawRectangle(dirtyRect);
    }
}