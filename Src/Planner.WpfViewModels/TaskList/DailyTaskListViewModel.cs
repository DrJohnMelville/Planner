using System;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using Melville.INPC;
using Melville.MVVM.AdvancedLists.PersistentLinq;
using Melville.MVVM.Wpf.Bindings;
using NodaTime;
using Planner.Models.Tasks;

namespace Planner.WpfViewModels.TaskList
{
    public interface IPlannerTaskFactory
    {
        PlannerTask Task(string title);
        PlannerTaskList TasksForDate(LocalDate date);
    }
    public partial class DailyTaskListViewModel
    {
        private readonly PlannerTaskList sourceList;
        public CollectionView TaskItems { get; }
        [AutoNotify] private bool isRankingTasks;
        [AutoNotify] private string newTaskName = "";

        private readonly IPlannerTaskFactory taskFactory;
        private readonly Func<PlannerTask, PlannerTaskViewModel> viewModelFactory;
        private readonly LocalDate date;
        public DailyTaskListViewModel(IPlannerTaskFactory taskFactory,
            Func<PlannerTask, PlannerTaskViewModel> viewModelFactory, LocalDate date)
        {
            this.taskFactory = taskFactory;
            this.viewModelFactory = viewModelFactory;
            this.date = date;
            sourceList = taskFactory.TasksForDate(date);
            TaskItems = CreateTaskItemsCollectionView();
            
        }

        private CollectionView CreateTaskItemsCollectionView()
        {
            var ret = new ListCollectionView(sourceList.SelectCol(viewModelFactory));
            ret.SortDescriptions.Add(new SortDescription("PlannerTask.Priority", ListSortDirection.Ascending));
            ret.SortDescriptions.Add(new SortDescription("PlannerTask.Order", ListSortDirection.Ascending));
            ret.IsLiveSorting = true;
            return ret;
        }

        public void ButtonA(PlannerTaskViewModel model) => PriorityButtonPress(model, 'A');
        public void ButtonB(PlannerTaskViewModel model) => PriorityButtonPress(model, 'B');
        public void ButtonC(PlannerTaskViewModel model) => PriorityButtonPress(model, 'C');
        public void ButtonD(PlannerTaskViewModel model) => PriorityButtonPress(model, 'D');

        private void PriorityButtonPress(PlannerTaskViewModel model, char button)
        {
            sourceList.PickPriority(model.PlannerTask, button);
            sourceList.SetImpliedOrders();
            CheckIfDonePrioritizing();
        }

        private void CheckIfDonePrioritizing()
        {
            if (sourceList.CompletelyPrioritized()) IsRankingTasks = false;
        }

        public void NewTaskKeyDown(Key key)
        {
            if (key == Key.Enter) TryAddPlannerTask();
        }
        public void TryAddPlannerTask()
        {
            if (!string.IsNullOrWhiteSpace(NewTaskName)) AddNewTask(NewTaskName);
            NewTaskName = "";
        }

        private void AddNewTask(string name) => sourceList.Add(taskFactory.Task(name));
    }
}