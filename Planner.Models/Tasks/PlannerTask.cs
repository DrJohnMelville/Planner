using System.Collections.Generic;

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
    public class PlannerTask
    {
        public string Name { get; set; } = "";
        public char Priority { get; set; } = ' ';
        public int Order { get; set; }
        public PlannerTaskStatus Status { get; set; }
        public string StatusDetail { get; set; } = "";
    }
}