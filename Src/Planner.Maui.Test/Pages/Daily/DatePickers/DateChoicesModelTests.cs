using Microsoft.Maui.Platform;
using NodaTime;
using Planner.Maui.Pages.Daily.DatePickers;

namespace Planner.Maui.Test.Pages.Daily.DatePickers;

public class DateChoicesModelTests
{
    [Theory]
    [InlineData(10,3,2024, 9,29, 11,2)]
    [InlineData(10,1,2024, 9,29, 11,2)]
    [InlineData(10,31,2024, 9,29, 11,2)]
    [InlineData(9,10,2024, 9,1, 10,5)]
    public void CreateMonthModel(int month, int day, int year, int firstMo, int firstDay,
        int lastMo, int lastDay)
    {
        var model = new MonthModel(new LocalDate(year, month, day));
        model.Items[0].FirstDate.Should().Be(new LocalDate(year, firstMo, firstDay));
        model.Items[^1].LastDate.Should().Be(new LocalDate(year, lastMo, lastDay));
    }

    [Fact]
    public void MonthModelName() => 
        new MonthModel(new LocalDate(2024, 10, 1)).Title.Should().Be("Oct 2024");

    [Fact]
    public void StepUpToYear()
    {
        var first = new MonthModel(new LocalDate(2024, 10, 1));
        var second = first.NextBiggerStep();
        second.Should().BeOfType<YearModel>();
        second.Title.Should().Be("2024");
        second.BaseDate.Should().Be(new LocalDate(2024, 1, 1));
        second.Items.Should().HaveCount(12);
        second.Items.Select(i=>i.FirstDate).Should().BeEquivalentTo(
            Enumerable.Range(1, 12).Select(i => new LocalDate(2024, i, 1)));
    }

}