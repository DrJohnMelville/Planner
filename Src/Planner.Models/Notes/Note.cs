using Melville.INPC;
using NodaTime;
using Planner.Models.Repositories;

namespace Planner.Models.Notes
{
    public partial class Note: PlannerItemBase
    {
        [AutoNotify] private LocalDate displaysOnDate; // a note does note have to be created on the date it is displayed upon.
        [AutoNotify] private Instant timeCreated;  
        [AutoNotify] private string name = "";
        [AutoNotify] private string text = "";
    }
}