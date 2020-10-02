using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Melville.MVVM.AdvancedLists;
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
        private readonly Mock<ILocalPlannerTaskRepository> taskFactory = new Mock<ILocalPlannerTaskRepository>();
        

        private readonly DailyTaskListViewModel sut;
        private readonly PlannerTaskViewModel itemVM;
        private readonly LocalDate date = new LocalDate(1975, 07, 28);

        public DailyTaskListViewModelTest()
        {
            taskFactory.Setup(i => i.CreateTask(It.IsAny<string>(), It.IsAny<LocalDate>())).Returns(
                (string s, LocalDate ld) => new PlannerTask() {Name = s});
            taskFactory.Setup(i => i.TasksForDate(date)).Returns(
                (Func<LocalDate, IList<PlannerTask>>)GenerateDailyTaskList);
                sut = new DailyTaskListViewModel(taskFactory.Object, i=>new PlannerTaskViewModel(i),
                date);
            itemVM = sut.TaskItems.OfType<PlannerTaskViewModel>().First();
        }
        public IList<PlannerTask> GenerateDailyTaskList(LocalDate date)
        {
            var src = new ThreadSafeBindableCollection<PlannerTask>();
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
            sut.NewTaskName = "Foo";
            sut.NewTaskKeyDown(Key.A);
            taskFactory.VerifyIgnoreArgs(i=>i.CreateTask(null, date), Times.Never);
            sut.NewTaskKeyDown(Key.Enter);
            taskFactory.Verify(i=>i.CreateTask("Foo", date), Times.Once);
            
        }


        [Fact]
        public void TryAddTask()
        {
            sut.NewTaskName = "Foo";
            taskFactory.VerifyIgnoreArgs(i=>i.CreateTask(null, date), Times.Never);
            sut.TryAddPlannerTask();
            taskFactory.Verify(i=>i.CreateTask("Foo", date), Times.Once);
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
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        public void TestDefer(int deferIndex)
        {
            var item = sut.TaskItems.OfType<PlannerTaskViewModel>().First();
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
            VerifyTaskDeferredToDate(targetDate, item.PlannerTask);
        }

        private void VerifyTaskDeferredToDate(LocalDate targetDate, PlannerTask task)
        {
            taskFactory.Verify(i => i.CreateTask(task.Name, targetDate), Times.Once);
            Assert.Equal(PlannerTaskStatus.Deferred, task.Status);
            Assert.Equal(targetDate.ToString("D", null), task.StatusDetail);
        }

        [Fact]
        public void DeferToDateTest()
        {
            var item = sut.TaskItems.OfType<PlannerTaskViewModel>().First();
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
            VerifyTaskDeferredToDate(localDate, item.PlannerTask);
            Assert.False(item.PopupOpen);
        }

    }
}