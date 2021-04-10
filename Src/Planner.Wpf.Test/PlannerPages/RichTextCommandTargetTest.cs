using System;
using System.Text.RegularExpressions;
using Melville.SystemInterface.RunShellCommands;
using Moq;
using NodaTime;
using Planner.Models.Tasks;
using Planner.Wpf.PlannerPages;
using Xunit;

namespace Planner.Wpf.Test.PlannerPages
{
    public class RichTextCommandTargetTest
    {
        private readonly Mock<IPlannerNavigator> navigation = new();
        private readonly Mock<IRunShellCommand> command = new();
        private RichTextCommandTarget sut;

        public RichTextCommandTargetTest()
        {
            sut = new RichTextCommandTarget(navigation.Object, command.Object,
                new LocalDate(1975, 07, 28));
        }

        [Fact]
        public void PlannerPagerLink3()
        {
            var match = Regex.Match("(1.2.3)", @"\((\d+)\.(\d+)\.(\d+)\)");
            sut.PlannerPageLinkClicked(new Segment<TaskTextType>("(1.2.3)", "(1.2.3)", TaskTextType.PlannerPage,
                match));
            navigation.Verify(i => i.ToDate(new LocalDate(1975, 1, 2)));

        }

        [Fact]
        public void PlannerPagerLink4DightYeat()
        {
            var match = Regex.Match("(1.2.1980.3)", @"\((\d+)\.(\d+)\.(\d+)\.(\d+)\)");
            sut.PlannerPageLinkClicked(new Segment<TaskTextType>("(1.2.1980.3)", "(1.2.1980.3)",
                TaskTextType.PlannerPage, match));
            navigation.Verify(i => i.ToDate(new LocalDate(1980, 1, 2)));
        }

        [Fact]
        public void PlannerPagerLink2DigitYear()
        {
            var match = Regex.Match("(1.2.80.3)", @"\((\d+)\.(\d+)\.(\d+)\.(\d+)\)");
            sut.PlannerPageLinkClicked(new Segment<TaskTextType>("(1.2.80.3)", "(1.2.80.3)", TaskTextType.PlannerPage,
                match));
            navigation.Verify(i => i.ToDate(new LocalDate(1980, 1, 2)));
        }

        [Fact]
        public void OpenWebLink()
        {
            sut.WebLinkLinkClicked(new Segment<TaskTextType>("www.google.com", TaskTextType.WebLink, 0));
            command.Verify(i => i.ShellExecute("www.google.com", Array.Empty<string>()));
            command.VerifyNoOtherCalls();
        }

        [Fact]
        public void OpenFileLink()
        {
            sut.FileLinkLinkClicked(new Segment<TaskTextType>("c:\\blah.txt", TaskTextType.FileLink, 0));
            command.Verify(i => i.ShellExecute("c:\\blah.txt", Array.Empty<string>()));
            command.VerifyNoOtherCalls();
        }
    }
}
