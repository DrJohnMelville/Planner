using Melville.INPC;

namespace Planner.Models.Login;

public partial class TargetSite
{
    [FromConstructor] public string Name { get; }
    [FromConstructor] public string Url { get; } 
}