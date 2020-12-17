using NodaTime;
using NodaTime.TimeZones;

namespace Planner.Models.Time
{
    public interface IUsersClock
    {
        DateTimeZone CurrentUiTimeZone();
        LocalDate CurrentDate();
    }
    public class UsersClock: IUsersClock
    {
        private IClock clock;

        public UsersClock(IClock clock)
        {
            this.clock = clock;
        }

        public DateTimeZone CurrentUiTimeZone() =>
            DateTimeZoneProviders.Tzdb.GetSystemDefault();

        public LocalDate CurrentDate() => 
            clock.GetCurrentInstant().InZone(CurrentUiTimeZone()).Date;
    }

    public static class UserClockOperations
    {
        public static LocalDateTime InstantToLocalDateTime(this IUsersClock clock, Instant instant) =>
            clock.InstantInZone(instant).LocalDateTime;
        public static ZonedDateTime InstantInZone(this IUsersClock clock, Instant instant) =>
            instant.InZone(clock.CurrentUiTimeZone());
    }
}