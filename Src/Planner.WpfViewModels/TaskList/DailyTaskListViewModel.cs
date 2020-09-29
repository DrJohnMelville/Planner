using System;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using Melville.INPC;
using Melville.MVVM.AdvancedLists.PersistentLinq;
using NodaTime;
using Planner.Models.Repositories;
using Planner.Models.Tasks;

namespace Planner.WpfViewModels.TaskList
{
    public partial class DailyTaskListViewModel
    {
        private readonly PlannerTaskList sourceList;
        public CollectionView TaskItems { get; }
        public string[] DeferToName { get; }

        [AutoNotify] private bool isRankingTasks;
        [AutoNotify] private string newTaskName = "";

        private readonly ILocalPlannerTaskRepository taskRepository;
        private readonly Func<PlannerTask, PlannerTaskViewModel> viewModelFactory;
        private readonly LocalDate date;
        public DailyTaskListViewModel(ILocalPlannerTaskRepository taskRepository,
            Func<PlannerTask, PlannerTaskViewModel> viewModelFactory, LocalDate date)
        {
            this.taskRepository = taskRepository;
            this.viewModelFactory = viewModelFactory;
            this.date = date;
            sourceList = taskRepository.TasksForDate(date);
            TaskItems = CreateTaskItemsCollectionView();
            DeferToName = CreateDayNames();
        }

        private string[] CreateDayNames()
        {
            var ret = new string[7];
            ret[0] = "Tomorrow";
            for (int i = 1; i < 7; i++)
            {
                ret[i] = date.PlusDays(i+1).DayOfWeek.ToString();
            }
            return ret;
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

        private void AddNewTask(string name) => sourceList.Add(taskRepository.CreateTask(name, date));

        public void Defer0(PlannerTaskViewModel task) => Defer(0, task);
        public void Defer1(PlannerTaskViewModel task) => Defer(1, task);
        public void Defer2(PlannerTaskViewModel task) => Defer(2, task);
        public void Defer3(PlannerTaskViewModel task) => Defer(3, task);
        public void Defer4(PlannerTaskViewModel task) => Defer(4, task);
        public void Defer5(PlannerTaskViewModel task) => Defer(5, task);
        public void Defer6(PlannerTaskViewModel task) => Defer(6, task);

        public void Defer(int index, PlannerTaskViewModel task) => 
            DeferTaskToDate(task.PlannerTask, date.PlusDays(index + 1));

        private void DeferTaskToDate(PlannerTask task, LocalDate targetDate)
        {
            taskRepository.CreateTask(task.Name, targetDate);
            task.Status = PlannerTaskStatus.Deferred;
            task.StatusDetail = targetDate.ToString("D", null);
        }

        public void DeferToDate(PlannerTaskViewModel item)
        {
            item.PopUpContent = new PickDeferDate(
                date, "Defer Task", CompleteDeferral);
            item.PopupOpen = true;
            
            void CompleteDeferral(LocalDate dd)
            {
                DeferTaskToDate(item.PlannerTask, dd);
                item.PopupOpen = false;
            }
        }
    }
}