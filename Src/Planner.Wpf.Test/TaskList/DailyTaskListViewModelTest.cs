using System;
using System.Linq;
using System.Windows.Input;
using Melville.MVVM.RunShellCommands;
using Melville.MVVM.Wpf.KeyboardFacade;
using Melville.TestHelpers.InpcTesting;
using Melville.TestHelpers.MockConstruction;
using Moq;
using NodaTime;
using Planner.Models.Repositories;
using Planner.Models.Tasks;
using Planner.WpfViewModels.TaskList;
using Xunit;

namespace Planner.Wpf.Test.TaskList
{
    public class DailyTaskListViewModelTest
    {
        private readonly Mock<IKeyboardQuery> keyboard = new();
        private readonly Mock<ILocalRepository<PlannerTask>> taskFactory = new();
        private readonly DailyTaskListViewModel sut;
        private readonly PlannerTaskViewModel itemVM;
        private readonly LocalDate date = new LocalDate(1975, 07, 28);

        public DailyTaskListViewModelTest()
        {
            taskFactory.Setup(i => i.CreateItem(It.IsAny<LocalDate>(), 
                It.IsAny<Action<PlannerTask>>())).Returns(
                valueFunction: (LocalDate ld, Action<PlannerTask> init) =>
                {
                    var ret = new PlannerTask();
                    init(ret);
                    return ret;
                });
            taskFactory.Setup(i => i.ItemsForDate(date)).Returns(
                (Func<LocalDate, IListPendingCompletion<PlannerTask>>)GenerateDailyTaskList);
            sut = new DailyTaskListViewModel(taskFactory.Object, i=>new PlannerTaskViewModel(i),
                keyboard.Object, date);
            itemVM = sut.TaskViewModels.OfType<PlannerTaskViewModel>().First();
        }
        public IListPendingCompletion<PlannerTask> GenerateDailyTaskList(LocalDate date)
        {
            var src = new ItemList<PlannerTask>();
            src.Add(new PlannerTask() {Name = "Task1"});
            src.Add(new PlannerTask() {Name = "Task2"});
            src.Add(new PlannerTask() {Name = "Task3"});
            src.Add(new PlannerTask() {Name = "Task4"});
            return src;
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
                i=>i.ShowPriorityButton, i=>i.ShowOrderButtons, 
                i=>i.ShowPriorityButton, i=>i.ShowOrderButtons);
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
            foreach (var model in sut.TaskViewModels.OfType<PlannerTaskViewModel>())
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
            var models = sut.TaskViewModels.OfType<PlannerTaskViewModel>().ToList();
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
            var models = sut.TaskViewModels.OfType<PlannerTaskViewModel>().ToList();
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
            Assert.Equal(4, sut.TaskViewModels.Count);
            Assert.Equal("", sut.NewTaskName);
        }

        [Fact]
        public void SetPriority()
        {
            var model = sut.TaskViewModels.OfType<PlannerTaskViewModel>().First();
            sut.SetItemPriority(model, new PriorityKey('D',3));
            Assert.Equal("D3", model.PlannerTask.PriorityDisplay);
        }

        [Fact]
        public void SetupPriorityMenu()
        {
            var model = sut.TaskViewModels.OfType<PlannerTaskViewModel>().First();
            Assert.Empty(model.Menus.OfType<object>());
            sut.InitializePriorityMenu(model);
            Assert.Equal(5, model.Menus.OfType<object>().Count());
        }



        [Fact]
        public void EnterTriesToCreateNewTask()
        {
            Assert.Equal(4, sut.TaskViewModels.Count);
            sut.NewTaskKeyDown(Key.Enter);
            sut.NewTaskName = "Foo";
            sut.NewTaskKeyDown(Key.A);
            taskFactory.VerifyIgnoreArgs(i=>i.CreateItem(date, 
                It.IsAny<Action<PlannerTask>>()), Times.Never);
            sut.NewTaskKeyDown(Key.Enter);
            taskFactory.Verify(i=>i.CreateItem(date, 
                It.IsAny<Action<PlannerTask>>()), Times.Once);
            
        }


        [Fact]
        public void TryAddTask()
        {
            sut.NewTaskName = "Foo";
            taskFactory.VerifyIgnoreArgs(i=>i.CreateItem(date, 
                It.IsAny<Action<PlannerTask>>()), Times.Never);
            sut.TryAddPlannerTask();
            taskFactory.Verify(i=>i.CreateItem(date, 
                It.IsAny<Action<PlannerTask>>()), Times.Once);
            Assert.Equal("", sut.NewTaskName);
        }

        [Theory]
        [InlineData(0,"Tomorrow")]
        [InlineData(1,"Wednesday")]
        [InlineData(2,"Thursday")]
        [InlineData(3,"Friday")]
        [InlineData(4,"Saturday")]
        [InlineData(5,"Sunday")]
        [InlineData(6,"Monday")]
        public void DeferDayName(int index, string label)
        {
            Assert.Equal(label, sut.DeferToName[index]);
        }

        [Theory]
        [InlineData(0, false, PlannerTaskStatus.Deferred)]
        [InlineData(1, false, PlannerTaskStatus.Deferred)]
        [InlineData(2, false, PlannerTaskStatus.Deferred)]
        [InlineData(3, false, PlannerTaskStatus.Deferred)]
        [InlineData(4, false, PlannerTaskStatus.Deferred)]
        [InlineData(5, false, PlannerTaskStatus.Deferred)]
        [InlineData(6, false, PlannerTaskStatus.Deferred)]
        [InlineData(0, true, PlannerTaskStatus.Done)]
        [InlineData(1, true, PlannerTaskStatus.Done)]
        [InlineData(2, true, PlannerTaskStatus.Done)]
        [InlineData(3, true, PlannerTaskStatus.Done)]
        [InlineData(4, true, PlannerTaskStatus.Done)]
        [InlineData(5, true, PlannerTaskStatus.Done)]
        [InlineData(6, true, PlannerTaskStatus.Done)]
        public void TestDefer(int deferIndex, bool ctrlDown, PlannerTaskStatus finalStatus)
        {
            if (ctrlDown) keyboard.SetupGet(i => i.Modifiers).Returns(ModifierKeys.Control);
            var item = sut.TaskViewModels.OfType<PlannerTaskViewModel>().First();
            switch (deferIndex)
            {
                case 0: sut.Defer0(item); break;
                case 1: sut.Defer1(item); break;
                case 2: sut.Defer2(item); break;
                case 3: sut.Defer3(item); break;
                case 4: sut.Defer4(item); break;
                case 5: sut.Defer5(item); break;
                case 6: sut.Defer6(item); break;
            }
            
            var targetDate = new LocalDate(1975, 07, 28).PlusDays(1 + deferIndex);
            VerifyTaskDeferredToDate(targetDate, item.PlannerTask, finalStatus);
        }

        private void VerifyTaskDeferredToDate(LocalDate targetDate, PlannerTask task,
            PlannerTaskStatus finalStatus)
        {
            taskFactory.Verify(i => i.CreateItem(targetDate, 
                It.IsAny<Action<PlannerTask>>()), Times.Once);
            Assert.Equal(finalStatus, task.Status);
            Assert.Equal(targetDate.ToString("D", null), task.StatusDetail);
        }

        [Fact]
        public void DeferToDateTest()
        {
            var item = sut.TaskViewModels.OfType<PlannerTaskViewModel>().First();
            sut.DeferToDate(item);
            
            // show the popup
            Assert.True(item.PopupOpen);
            Assert.True(item.PopUpContent is PickDeferDate);
            var po = (PickDeferDate) item.PopUpContent;
            Assert.Equal("Defer Task", po.ButtonLabel);
            var localDate = new LocalDate(2020,07,28);
            
            //pick a date
            po.SelectedDate = localDate;
            po.DoDeferral();
            VerifyTaskDeferredToDate(localDate, item.PlannerTask, PlannerTaskStatus.Deferred);
            Assert.False(item.PopupOpen);
        }

        [Fact]
        public void OpenWebLink()
        {
            var command = new Mock<IRunShellCommand>();
            sut.WebLinkLinkClicked(new Segment<TaskTextType>("www.google.com", TaskTextType.WebLink, 0),
                command.Object);
            command.Verify(i=>i.ShellExecute("www.google.com", Array.Empty<string>()));
            command.VerifyNoOtherCalls();
        }
        [Fact]
        public void OpenFileLink()
        {
            var command = new Mock<IRunShellCommand>();
            sut.FileLinkLinkClicked(new Segment<TaskTextType>("c:\\blah.txt", TaskTextType.FileLink, 0),
                command.Object);
            command.Verify(i=>i.ShellExecute("c:\\blah.txt", Array.Empty<string>()));
            command.VerifyNoOtherCalls();
        }

        [Fact]
        public void DeleteTask()
        {
            var item = sut.TaskViewModels.OfType<PlannerTaskViewModel>().First();
            sut.DeleteTask(item);
            Assert.False(sut.TaskViewModels.Contains(item));
        }

    }
}