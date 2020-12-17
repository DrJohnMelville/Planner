using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Melville.MVVM.CSharpHacks;
using Microsoft.Office.Interop.Outlook;
using NodaTime;
using NodaTime.Extensions;
using Planner.Models.Appointments.SyncStructure;
using Planner.Models.HtmlGeneration;
using Planner.Models.Repositories;

namespace Planner.OutlookInterop
{
 public class OutlookSyncMonitor
    {
        private readonly Folder appointments;
        private readonly Items appointmentItemsKeepAlive; // Must be kept alive to provide change notifications.
        private readonly Folder deletedItems;
        private readonly IAppointmentSyncEngine engine;
        private readonly IClock clock;
        private readonly IEventBroadcast<ClearCachesEventArgs> clearCaches;

        public OutlookSyncMonitor(
            IAppointmentSyncEngine engine, 
            IClock clock, 
            string account, 
            IEventBroadcast<ClearCachesEventArgs> clearCaches)
        {
            this.engine = engine;
            this.clock = clock;
            this.clearCaches = clearCaches;
            (appointments, deletedItems) = ConnectToOutlook(account);
            appointmentItemsKeepAlive = MonitorSourceForChanges();
        }

        private (Folder, Folder) ConnectToOutlook(string account)
        {
            var nameSpace = new Application().GetNamespace("MAPI");
            var appointmentFolder = nameSpace.FolderByPath(account+@"\Calendar") ?? 
                           throw new InvalidOperationException("Cannot find calendar folder");
            var deletedItemsFolder = nameSpace.FolderByPath(account+@"\Deleted Items") ??
                           throw new InvalidOperationException("Cannot find deleted items folder");
            return (appointmentFolder, deletedItemsFolder);
        }

        private Items MonitorSourceForChanges()
        {
            var aptList = appointments.Items;
            aptList.ItemAdd += LaunchAppointmentSync;
            aptList.ItemRemove +=  () => LaunchAppointmentSync("");
            aptList.ItemChange += LaunchAppointmentSync;
            return aptList;
        }

        private void LaunchAppointmentSync(object _) => DoSync().FireAndForget();

        public async Task DoSync()
        {
            await engine.Synchronize(
                CreateSyncObject(DateOneYearFromNow(), await engine.LastSynchronizationTime())
            );
            clearCaches.Fire(this, new ClearCachesEventArgs());
        }

        private AppointmentSyncInfo CreateSyncObject(LocalDate endDate, Instant lastSynchronizationTime) =>
            new()
            {
                Items = NewOrChangedItemsQuery(endDate, lastSynchronizationTime),
                KeysToDelete = DeletedItemsQuery(endDate, lastSynchronizationTime)
            };

        private List<SyncAppointmentData> NewOrChangedItemsQuery(LocalDate endDate, Instant lastSynchronizationTime) =>
            appointments.AppointmentItemsFromFolder(endDate, lastSynchronizationTime)
                .GetItems<AppointmentItem>()
                .GroupBy(i => i.GlobalAppointmentID)
                .Select(CreateSyncAppointData)
                .ToList();

        private List<string> DeletedItemsQuery(LocalDate endDate, Instant lastSynchronizationTime) =>
            deletedItems.AppointmentItemsFromFolder(endDate, lastSynchronizationTime)
                .GetItems<AppointmentItem>()
                .Select(i => i.GlobalAppointmentID)
                .Distinct().ToList();

        private SyncAppointmentData CreateSyncAppointData(IGrouping<string, AppointmentItem> i)
        {
            var appointmentItem = i.First();
            return new()
            {
                BodyText = appointmentItem.Body,
                Title = appointmentItem.Subject,
                Location = appointmentItem.Location,
                UniqueOutlookId = i.Key,
                Times = i.Select(CreateAppointmentTime).ToList()
            };
        }

        private SyncAppointmentTime CreateAppointmentTime(AppointmentItem j) => 
            new() {StartTime = UtcToInstant(j.StartUTC), EndTime = UtcToInstant(j.EndUTC)};

        private static Instant UtcToInstant(DateTime dateTime) => 
            DateTime.SpecifyKind(dateTime, DateTimeKind.Utc).ToInstant();

        private LocalDate DateOneYearFromNow() =>
            clock.GetCurrentInstant().InUtc()
                .LocalDateTime.PlusYears(1).Date;
    }
}