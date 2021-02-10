using System.Globalization;
using Moq;
using NodaTime;
using Planner.Models.Time;
using Planner.Wpf.Notes;
using Xunit;

namespace Planner.Wpf.Test.Notes
{
    public class TimeDisplayConverterTest
    {
        private readonly Mock<IUsersClock> clock = new();
        private readonly TimeDisplayConverter sut;

        public TimeDisplayConverterTest()
        {
            clock.Setup(i => i.CurrentUiTimeZone()).Returns(DateTimeZone.Utc);
            sut = new TimeDisplayConverter() {UsersClock = clock.Object};
        }

        [Theory]
        [InlineData(null, "7/28/1975 1:01 AM")]
        [InlineData("{0:dddd}", "Monday")]
        public void DisplayBaseTime(string format, string result)
        {
            Assert.Equal(result, sut.Convert(Instant.FromUtc(1975,07,28, 1,1),
                typeof(string), format, CultureInfo.CurrentCulture));
            
        }

    }
}