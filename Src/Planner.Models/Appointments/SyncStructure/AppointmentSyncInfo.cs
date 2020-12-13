using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Planner.Models.Appointments.SyncStructure
{
    public class AppointmentSyncInfo
    {
        public IList<string> KeysToDelete { get; set; } = Array.Empty<string>();
        public IList<SyncAppointmentData> Items { get; set; } = Array.Empty<SyncAppointmentData>();
        
        public List<string> DeletedAndModifiedItemOutlookIds() =>
            KeysToDelete.Concat(OutlookIdsForPotentiallyModifiedItems()).ToList();

        private IEnumerable<string> OutlookIdsForPotentiallyModifiedItems() =>
            Items
                .Select(i=>i.UniqueOutlookId)
                .Where(i=>!string.IsNullOrWhiteSpace(i));
    }
}