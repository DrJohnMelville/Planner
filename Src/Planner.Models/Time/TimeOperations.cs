using System;
using NodaTime;

namespace Planner.Models.Time
{
    public static class TimeOperations
    {
        public static LocalDate CurrentDate(this IClock clock) => clock.GetCurrentInstant()
                .InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault())
                .LocalDateTime.Date;

        public static bool TryParseLocalDate(string s, out LocalDate ret)
        {
            if (DateTime.TryParse(s, out var dt))
            {
                ret = LocalDate.FromDateTime(dt);
                return true;
            }
            ret = LocalDate.MinIsoValue;
            return false;
        }
    }
}