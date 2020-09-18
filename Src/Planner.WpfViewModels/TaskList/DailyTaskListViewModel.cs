using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Melville.INPC;
using Melville.MVVM.AdvancedLists.PersistentLinq;
using Planner.Models.Tasks;

namespace Planner.WpfViewModels.TaskList
{
    public interface IPlannerTaskFactory
    {
        PlannerTask Task(string title);
    }
    public partial class DailyTaskListViewModel
    {
        private readonly PlannerTaskList sourceList;
        public CollectionView TaskItems { get; }
        [AutoNotify] private bool isRankingTasks;
        [AutoNotify] private string newTaskName;
        

        private static void AddFakeData(PlannerTaskList src)
        {
            src.Add(new PlannerTask() {Name = "Task1"});
            src.Add(new PlannerTask() {Name = "Task2"});
            src.Add(new PlannerTask() {Name = "Task3"});
            src.Add(new PlannerTask() {Name = "Task4"});
        }

        private readonly IPlannerTaskFactory taskFactory;
        public DailyTaskListViewModel(IPlannerTaskFactory taskFactory)
        {
            this.taskFactory = taskFactory;
            sourceList = new PlannerTaskList();
            AddFakeData(sourceList);
            TaskItems = CreateTaskItemsCollectionView();
            
        }

        private CollectionView CreateTaskItemsCollectionView()
        {
            var ret = new ListCollectionView(sourceList.SelectCol(i => new PlannerTaskViewModel(i)));
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
            TryAutoOrder();
            CheckIfDonePrioritizing();
        }

        private void TryAutoOrder()
        {
            if (AnyTaskLacksAPriority()) return;
            foreach (var task in SingleUnassignedTasksWithinTheirPriority())
            {
                sourceList.PickPriority(task.PlannerTask, 'A');                              
            }
        }

        private IEnumerable<PlannerTaskViewModel> SingleUnassignedTasksWithinTheirPriority() =>
            TaskItems
                .OfType<PlannerTaskViewModel>()
                .Where(i=>i.PlannerTask.Order < 1)
                .GroupBy(i=>i.PlannerTask.Priority)
                .Where(i=>i.Count() == 1)
                .Select(i=>i.First());

        private bool AnyTaskLacksAPriority() => TaskItems.OfType<PlannerTaskViewModel>().Any(i => i.PlannerTask.Priority == ' ');

        private void CheckIfDonePrioritizing()
        {
            if (sourceList.All(i => i.Prioritized)) IsRankingTasks = false;
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