using System.Collections.Generic;
using System.Windows.Data;
using Melville.MVVM.AdvancedLists.PersistentLinq;
using Planner.Models.Tasks;
using Melville.Generated;

namespace Planner.Wpf.TaskList
{
    public class DailyTaskListViewModel
    {
        private readonly PlannerTaskList sourceList;
        public CollectionView TaskItems { get; }
        private bool isRankingTasks;

        private static void AddFakeData(PlannerTaskList src)
        {
            src.Add(new PlannerTask() {Name = "Task1"});
            src.Add(new PlannerTask() {Name = "Task2"});
            src.Add(new PlannerTask() {Name = "Task3"});
            src.Add(new PlannerTask() {Name = "Task4"});
        }

        public DailyTaskListViewModel()
        {
            var src = new PlannerTaskList();
            AddFakeData(src);
            TaskItems = new ListCollectionView(src.SelectCol(i => new PlannerTaskViewModel(i)));
        }
    }

    public class PlannerTaskViewModel
    {
        public PlannerTask PlannerTask {get;}

        public PlannerTaskViewModel(PlannerTask plannerTask)
        {
            PlannerTask = plannerTask;
        }
    }
}