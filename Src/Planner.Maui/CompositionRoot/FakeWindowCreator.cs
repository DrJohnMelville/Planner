using Melville.INPC;

namespace Planner.Maui.CompositionRoot;

[StaticSingleton]
public partial class FakeWindowCreator : IWindowCreator
{
    public Window CreateWindow(Application app, IActivationState? activationState) => 
        null!;
}