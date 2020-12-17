using System;
using Melville.MVVM.RunShellCommands;
using NodaTime;
using Planner.Models.Tasks;

namespace Planner.WpfViewModels.PlannerPages
{
    public class RichTextCommandTarget
    {
        private readonly IPlannerNavigator navigator;
        private readonly IRunShellCommand commandObject;
        private readonly LocalDate currentDate;

        public RichTextCommandTarget(
            IPlannerNavigator navigator, IRunShellCommand commandObject, LocalDate currentDate)
        {
            this.navigator = navigator;
            this.commandObject = commandObject;
            this.currentDate = currentDate;
        }
        public void PlannerPageLinkClicked(Segment<TaskTextType> segment)
        {
            if (segment.Match == null) return;
            navigator.NavigateToDate(segment.Match.Groups, currentDate);
        }
        public void WebLinkLinkClicked(Segment<TaskTextType> segment) =>
            commandObject.ShellExecute(segment.Text, Array.Empty<string>());
        public void FileLinkLinkClicked(
            Segment<TaskTextType> segment) =>
            commandObject.ShellExecute(segment.Text, Array.Empty<string>());

    }
}