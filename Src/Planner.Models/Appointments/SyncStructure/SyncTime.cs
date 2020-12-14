using System;
using NodaTime;

namespace Planner.Models.Appointments.SyncStructure
{
    public class SyncTime
    {
        public Guid SyncTimeId { get; set; }
        public Instant Time { get; set; }
    }
}