using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Outlook;
using NodaTime;

namespace Planner.OutlookInterop
{
    public static class MapiOperations
    {
        public static Items AppointmentItemsFromFolder(this Folder folder, LocalDate endDate,
            Instant lastUpdateInstant)
        {
            var items = folder.Items.Restrict(
                $"[Start] < '{endDate:MM/dd/yyyy}' and [LastModificationTime] > '{LastUpdateInLocalTime(lastUpdateInstant):MM/dd/yyyy hh:mm tt}'");
            items.Sort("[Start]");
            items.IncludeRecurrences = true;
            return items;
        }

        // The Exchange data model states that LastModificationTime is stored in UTC, but the client very politely
        // converts it to local time for us.  So we express our query constraint in local time as well.
        private static ZonedDateTime LastUpdateInLocalTime(Instant lastUpdateInstant) => 
            lastUpdateInstant.InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault());

        public static Folder? FolderByPath(this NameSpace ns, string path) =>
            ByPath(ns.Folders.OfType<Folder>(),
                path.Split(new[] {'\\', '/'}, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries), 0);
        public static Folder? ByPath(IEnumerable<Folder> fols, string[] keys, int position)
        {
            var list = fols;
            while (true)
            {
                var folder = list?.First(i => i.Name == keys[position]);
                position++;
                if (position == keys.Length) return folder;
                list = folder?.Folders.OfType<Folder>();
            }
        }

        public static IEnumerable<T> GetItems<T>(this Items folder)
        {
            var itemsCount = folder.Count;
            for (int i = 0; i < itemsCount; i++)
            {
                var item = folder[i+1];
                if (item == null) yield break;
                if (item is T output) yield return output;
            }
        }
    }
}