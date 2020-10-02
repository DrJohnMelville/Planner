using Melville.INPC;
using NodaTime;
using Planner.Models.Repositories;

namespace Planner.Models.Notes
{
    public partial class Note: PlannerItemWithDate
    {
        [AutoNotify] private Instant timeCreated;  
        [AutoNotify] private string name = "";
        [AutoNotify] private string text = "";
    }
}