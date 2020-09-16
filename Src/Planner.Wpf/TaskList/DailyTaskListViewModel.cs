using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Melville.MVVM.AdvancedLists.PersistentLinq;
using Planner.Models.Tasks;
using Melville.INPC;

namespace Planner.Wpf.TaskList
{
    public partial class DailyTaskListViewModel
    {
        private readonly PlannerTaskList sourceList;
        public CollectionView TaskItems { get; }
        [AutoNotify]private bool isRankingTasks;

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
            TaskItems.SortDescriptions.Add(new SortDescription("Priority", ListSortDirection.Ascending));
            TaskItems.SortDescriptions.Add(new SortDescription("Order", ListSortDirection.Ascending));
            TaskItems.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            ((ICollectionViewLiveShaping) TaskItems).IsLiveSorting = true;
        }
        public void ButtonA(PlannerTaskViewModel model) => sourceList.PickPriority(model.PlannerTask, 'A');
        public void ButtonB(PlannerTaskViewModel model) => sourceList.PickPriority(model.PlannerTask, 'B');
        public void ButtonC(PlannerTaskViewModel model) => sourceList.PickPriority(model.PlannerTask, 'C');
        public void ButtonD(PlannerTaskViewModel model) => sourceList.PickPriority(model.PlannerTask, 'D');

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