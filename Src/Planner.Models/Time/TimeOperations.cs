using NodaTime;

namespace Planner.Models.Time
{
    public static class TimeOperations
    {
        public static LocalDate CurrentDate(this IClock clock) => clock.GetCurrentInstant()
                .InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault())
                .LocalDateTime.Date;
    }
}