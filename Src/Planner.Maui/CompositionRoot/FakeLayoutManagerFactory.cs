using Melville.INPC;
using Microsoft.Maui.Layouts;

namespace Planner.Maui.CompositionRoot;

[StaticSingleton]
public partial class FakeLayoutManagerFactory: ILayoutManagerFactory
{
    public ILayoutManager CreateLayoutManager(Layout layout) => null!;
}