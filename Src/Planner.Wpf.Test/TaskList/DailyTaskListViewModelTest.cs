using System.Linq;
using System.Windows;
using Melville.TestHelpers.InpcTesting;
using Planner.Wpf.TaskList;
using Xunit;

namespace Planner.Wpf.Test.TaskList
{
    public class DailyTaskListViewModelTest
    {
        private readonly DailyTaskListViewModel sut = new DailyTaskListViewModel();
        private readonly PlannerTaskViewModel itemVM;

        public DailyTaskListViewModelTest()
        {
            itemVM = sut.TaskItems.OfType<PlannerTaskViewModel>().First();
        }

        [Theory]
        [InlineData(0, "A1")]
        [InlineData(1, "B1")]
        [InlineData(2, "C1")]
        [InlineData(3, "D1")]
        public void ShowPriorityButtons(int pickButton, string priority)
        {
            Assert.True(itemVM.ShowPriorityButton);
            using var _ = INPCCounter.VerifyInpcFired(itemVM, 
                i=>i.ShowPriorityButton, i=>i.ShowBlankButton, 
                i=>i.ShowPriorityButton, i=>i.ShowBlankButton);
            switch (pickButton)
            {
                case 0: sut.ButtonA(itemVM); break;
                case 1: sut.ButtonB(itemVM); break;
                case 2: sut.ButtonC(itemVM); break;
                case 3: sut.ButtonD(itemVM); break;
            }
            Assert.False(itemVM.ShowPriorityButton);
            sut.ButtonA(itemVM);
            Assert.Equal(priority, itemVM.PlannerTask.PriorityDisplay);
        }

        [Fact]
        public void TurnOffSortingWhenDone()
        {
            sut.IsRankingTasks = true;
            foreach (var model in sut.TaskItems.OfType<PlannerTaskViewModel>())
            {
                while (!model.PlannerTask.Prioritized)
                {
                    sut.ButtonA(model);
                }
            }
            Assert.False(sut.IsRankingTasks);
        }


    }
}