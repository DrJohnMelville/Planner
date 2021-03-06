﻿using System;
using Melville.INPC;
using Planner.Models.Repositories;


namespace Planner.Models.Tasks
{
    public enum PlannerTaskStatus
    {
        Incomplete = 0,
        Done = 1,
        Deferred = 2,
        Delegated =3,
        Pending = 4,
        Canceled = 5
    }
    public partial class PlannerTask: PlannerItemWithDate, IComparable<PlannerTask>, IComparable
    {
        [AutoNotify] private string name = "";
        [AutoNotify] private char priority  = ' ';
        [AutoNotify] private int order;
        [AutoNotify] private PlannerTaskStatus status; 
        [AutoNotify] private string statusDetail  = "";
        [AutoNotify] public string PriorityDisplay => $"{Priority}{DisplayedOrder(Order)}";
        [AutoNotify] public bool Prioritized => Priority != ' ' && Order > 0;

        public PlannerTask():this (Guid.Empty) { }

        public PlannerTask(Guid key) => Key = key;

        private string DisplayedOrder(int o) => o > 0 ? o.ToString() : "";
        
        public int CompareTo(PlannerTask? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return (priority.CompareTo(other.priority),
                    order.CompareTo(other.order),
                    String.Compare(name, other.name, StringComparison.CurrentCultureIgnoreCase)) switch
                {
                    (0, 0, var byName) => byName,
                    (0, var byOrder, _) => byOrder,
                    var (byPriority, _, _) => byPriority
                };
        }

        public int CompareTo(object? obj) => (obj is PlannerTask other) ? CompareTo(other) : -1;

        public void ToggleStatus() =>
            Status = Status switch
            {
                PlannerTaskStatus.Incomplete => PlannerTaskStatus.Done,
                PlannerTaskStatus.Done => PlannerTaskStatus.Pending,
                PlannerTaskStatus.Pending => PlannerTaskStatus.Canceled,
                PlannerTaskStatus.Delegated => PlannerTaskStatus.Done,
                PlannerTaskStatus.Deferred =>PlannerTaskStatus.Deferred,
                _=> PlannerTaskStatus.Incomplete
            };
    }
}