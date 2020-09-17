using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
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
            TaskItems = new ListCollectionView(sourceList.SelectCol(i => new PlannerTaskViewModel(i)));
            var lsCol = (ICollectionViewLiveShaping) TaskItems;
            TaskItems.SortDescriptions.Add(new SortDescription("PlannerTask.Priority", ListSortDirection.Ascending));
            TaskItems.SortDescriptions.Add(new SortDescription("PlannerTask.Order", ListSortDirection.Ascending));
            lsCol.IsLiveSorting = true;
        }
        public void ButtonA(PlannerTaskViewModel model) => PriorityButtonPress(model, 'A');
        public void ButtonB(PlannerTaskViewModel model) => PriorityButtonPress(model, 'B');
        public void ButtonC(PlannerTaskViewModel model) => PriorityButtonPress(model, 'C');
        public void ButtonD(PlannerTaskViewModel model) => PriorityButtonPress(model, 'D');

        private void PriorityButtonPress(PlannerTaskViewModel model, char button)
        {
            sourceList.PickPriority(model.PlannerTask, button);
            CheckIfDonePrioritizing();
        }

        private void CheckIfDonePrioritizing()
        {
            if (sourceList.All(i => i.Prioritized)) IsRankingTasks = false;
        }
    }
    
    [AutoNotify]
    public partial class PlannerTaskViewModel
    {
        public PlannerTask PlannerTask {get;}

        public PlannerTaskViewModel(PlannerTask plannerTask)
        {
            PlannerTask = plannerTask;
            ((IExternalNotifyPropertyChanged)this)
                .DelegatePropertyChangeFrom(PlannerTask, nameof(PlannerTask.PriorityDisplay),
            nameof(ShowPriorityButton), nameof(ShowBlankButton));
        }
        
        public bool ShowPriorityButton => PlannerTask.Priority == ' ';
        public bool ShowBlankButton => !ShowPriorityButton;
    }
    
    public class AndVisibilityConverter: IMultiValueConverter
    {
        public static readonly AndVisibilityConverter Instance = new AndVisibilityConverter(); 
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => 
            values?.All(i => i is bool b && b)??false ? Visibility.Visible : Visibility.Collapsed;

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}