using System.Linq;
using System.Windows.Input;
using Melville.TestHelpers.InpcTesting;
using Moq;
using Planner.Models.Tasks;
using Planner.WpfViewModels.TaskList;
using Xunit;

namespace Planner.Wpf.Test.TaskList
{
    public class DailyTaskListViewModelTest
    {
        private readonly Mock<IPlannerTaskFactory> taskFactory = new Mock<IPlannerTaskFactory>();
        

        private readonly DailyTaskListViewModel sut;
        private readonly PlannerTaskViewModel itemVM;

        public DailyTaskListViewModelTest()
        {
            taskFactory.Setup(i => i.Task(It.IsAny<string>())).Returns(
                (string s) => new PlannerTask() {Name = s});
            sut = new DailyTaskListViewModel(taskFactory.Object);
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

        [Fact]
        public void AutoOrder()
        {
            sut.IsRankingTasks = true;
            var models = sut.TaskItems.OfType<PlannerTaskViewModel>().ToList();
            Assert.Equal(4, models.Count);
            
            sut.ButtonA(models[0]);
            sut.ButtonB(models[1]);
            sut.ButtonC(models[2]);
            sut.ButtonD(models[3]);
            Assert.False(sut.IsRankingTasks);
            foreach (var model in models)
            {
                Assert.Equal(1, model.PlannerTask.Order);
            }            
        }
        [Fact]
        public void AutoOrderPartial()
        {
            sut.IsRankingTasks = true;
            var models = sut.TaskItems.OfType<PlannerTaskViewModel>().ToList();
            Assert.Equal(4, models.Count);
            
            sut.ButtonA(models[0]);
            sut.ButtonB(models[1]);
            sut.ButtonB(models[2]);
            sut.ButtonD(models[3]);
            Assert.True(sut.IsRankingTasks);
            Assert.Equal(1, models[0].PlannerTask.Order);
            Assert.Equal(0, models[1].PlannerTask.Order);
            Assert.Equal(0, models[2].PlannerTask.Order);
            Assert.Equal(1, models[3].PlannerTask.Order);

            sut.ButtonB(models[1]);
            
            Assert.False(sut.IsRankingTasks);
            Assert.Equal(1, models[0].PlannerTask.Order);
            Assert.Equal(1, models[1].PlannerTask.Order);
            Assert.Equal(2, models[2].PlannerTask.Order);
            Assert.Equal(1, models[3].PlannerTask.Order);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void TryAddNothing(string newName)
        {
            sut.NewTaskName = newName;
            sut.TryAddPlannerTask();
            Assert.Equal(4, sut.TaskItems.Count);
            Assert.Equal("", sut.NewTaskName);
            
        }

        [Fact]
        public void EnterTriesToCreateNewTask()
        {
            Assert.Equal(4, sut.TaskItems.Count);
            sut.NewTaskKeyDown(Key.Enter);
            Assert.Equal(4, sut.TaskItems.Count);
            sut.NewTaskName = "Foo";
            sut.NewTaskKeyDown(Key.A);
            Assert.Equal(4, sut.TaskItems.Count);
            sut.NewTaskKeyDown(Key.Enter);
            Assert.Equal(5, sut.TaskItems.Count);
            
        }


        [Fact]
        public void TryAddTask()
        {
            sut.NewTaskName = "Foo";
            sut.TryAddPlannerTask();
            Assert.Equal(5, sut.TaskItems.Count);
            Assert.Equal("Foo", sut.TaskItems.OfType<PlannerTaskViewModel>().First().PlannerTask.Name);
            Assert.Equal("", sut.NewTaskName);
        }


    }
}