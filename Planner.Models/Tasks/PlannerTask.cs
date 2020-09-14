using Melville.Generated;
using NodaTime;


namespace Planner.Models.Tasks
{
    public enum PlannerTaskStatus
    {
        Incomplete = 0,
        Done = 1,
        Deferred = 2,
        Delegated =3,
        Pending = 4,
        Cancelled = 5
    }
    public partial class PlannerTask
    {
        [AutoNotify] private LocalDate date;
        [AutoNotify] private string name = "";
        [AutoNotify] private char priority  = ' ';
        [AutoNotify] private int order;
        [AutoNotify] private PlannerTaskStatus status; 
        [AutoNotify] private string statusDetail  = "";
        [AutoNotify] public string PriorityDisplay => $"{Priority}{DisplayedOrder(Order)}";
        private string DisplayedOrder(int o) => o > 0 ? o.ToString() : "";
    }
}