using Microsoft.Maui.Platform;
using NodaTime;
using Planner.Maui.Pages.Daily.DatePickers;

namespace Planner.Maui.Test.Pages.Daily.DatePickers;

public class DateChoicesModelTests
{
    private readonly Mock<IDatePickerContext> contextMock = new();

    [Theory]
    [InlineData(10,3,2024, 9,29, 11,2)]
    [InlineData(10,1,2024, 9,29, 11,2)]
    [InlineData(10,31,2024, 9,29, 11,2)]
    [InlineData(9,10,2024, 9,1, 10,5)]
    public void CreateMonthModel(int month, int day, int year, int firstMo, int firstDay,
        int lastMo, int lastDay)
    {
        var model = NewMonthModel(year, month, day);
        model.Items[0].FirstDate.Should().Be(new LocalDate(year, firstMo, firstDay));
        model.Items[^1].LastDate.Should().Be(new LocalDate(year, lastMo, lastDay));
    }

    private MonthModel NewMonthModel(int year, int month, int day) => 
        new(new LocalDate(year, month, day), contextMock.Object);

    [Fact]
    public void MonthModelName() => 
         NewMonthModel(2024,10, 1).Title.Should().Be("Oct 2024");

    [Fact]
    public void SwipeMonthLeft()
    {
        var model = NewMonthModel(2024, 10, 1).PriorStep();
        model.Should().BeOfType<MonthModel>();
        model.Title.Should().Be("Sep 2024");
    }

    [Theory]
    [InlineData("", "Oct 2024")]
    [InlineData("U", "2024")]
    [InlineData("U0", "Jan 2024")]
    [InlineData("U1", "Feb 2024")]
    [InlineData("R", "Sep 2024")]
    [InlineData("L", "Nov 2024")]
    [InlineData("UU", "2020s")]
    [InlineData("UUR", "2010s")]
    [InlineData("UUL", "2030s")]
    [InlineData("UUL0", "2029")]
    [InlineData("UUL1", "2030")]
    [InlineData("UUU", "Century 2000")]
    [InlineData("UUUR", "Century 1900")]
    [InlineData("UUUL", "Century 2100")]
    [InlineData("UUUL1", "Century 2100s")]
    public void SwipeTest(string actions, string finalTitle)
    {
        DateChoicesModel model = NewMonthModel(2024, 10, 1);
        contextMock.Setup(i => i.SelectOptions(It.IsAny<DateChoicesModel>()))
            .Callback<DateChoicesModel>(m => model = m);
        foreach (var action in actions)
        {
            switch (action)
            {
                case 'L':
                    model.LeftCommand.Execute("");
                    break;
                case 'R':
                    model.RightCommand.Execute("");
                    break;
                case 'U':
                    model.UpCommand.Execute("");
                    break;
                case >= '0' and <= '9':
                    model.Items[action - '0'].DownCommand.Execute("");
                    break;
            }
        }

        model.Title.Should().Be(finalTitle);
    }

    [Fact]
    public void StepUpToYear()
    {
        var first = NewMonthModel(2024, 10, 1);
        var second = first.NextBiggerStep();
        second.Should().BeOfType<YearModel>();
        second.Title.Should().Be("2024");
        second.BaseDate.Should().Be(new LocalDate(2024, 1, 1));
        second.Items.Should().HaveCount(12);
        second.Items.Select(i=>i.FirstDate).Should().BeEquivalentTo(
            Enumerable.Range(1, 12).Select(i => new LocalDate(2024, i, 1)));
    }

}