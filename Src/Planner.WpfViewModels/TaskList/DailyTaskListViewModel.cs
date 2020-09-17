using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Melville.INPC;
using Melville.MVVM.AdvancedLists.PersistentLinq;
using Planner.Models.Tasks;

namespace Planner.WpfViewModels.TaskList
{
    public partial class DailyTaskListViewModel
    {
        private readonly PlannerTaskList sourceList;
        public CollectionView TaskItems { get; }
        [AutoNotify] private bool isRankingTasks;

        private static void AddFakeData(PlannerTaskList src)
        {
            src.Add(new PlannerTask() {Name = "Task1"});
            src.Add(new PlannerTask() {Name = "Task2"});
            src.Add(new PlannerTask() {Name = "Task3"});
            src.Add(new PlannerTask() {Name = "Task4"});
        }

        public DailyTaskListViewModel()
        {
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
    }
}