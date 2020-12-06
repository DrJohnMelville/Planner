using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Melville.INPC;
using Melville.MVVM.Wpf.Bindings;
using Planner.Models.Tasks;

namespace Planner.WpfViewModels.TaskList
{
    [AutoNotify]
    public partial class PlannerTaskViewModel
    {
        public PlannerTask PlannerTask {get;}
        public PlannerTaskViewModel(PlannerTask plannerTask)
        {
            PlannerTask = plannerTask;
            ((IExternalNotifyPropertyChanged)this)
                .DelegatePropertyChangeFrom(PlannerTask, nameof(PlannerTask.PriorityDisplay),
                    nameof(ShowPriorityButton), nameof(ShowOrderButtons));
            
            ((IExternalNotifyPropertyChanged)this).DelegatePropertyChangeFrom(PlannerTask,
                nameof(PlannerTask.Status), nameof(StatusDisplayText), nameof(StatusDisplayFont));

        }

        [AutoNotify] private IEnumerable<PriorityKey> menus = Array.Empty<PriorityKey>();
        [AutoNotify] public IEnumerable<PriorityKey> DigitMenu => 
            this.Menus.Where(i => i.Priority == PlannerTask.Priority)
                .Reverse();
        
        public bool ShowPriorityButton => PlannerTask.Priority == ' ';
        public bool ShowOrderButtons => !ShowPriorityButton && PlannerTask.Order == 0;
        public void MarkIncomplete() => PlannerTask.Status = PlannerTaskStatus.Incomplete;
        public void MarkDone() => PlannerTask.Status = PlannerTaskStatus.Done;
        public void MarkCanceled() => PlannerTask.Status = PlannerTaskStatus.Canceled;
        public void MarkPending() => ShowStatusPopup(PlannerTaskStatus.Pending, 
            new DelegatedContext(this, "Task Waiting For:"));
        public void MarkDelegated() => 
            ShowStatusPopup(PlannerTaskStatus.Delegated, 
                new DelegatedContext(this, "Task Delegated To:"));

        private void ShowStatusPopup(PlannerTaskStatus status, DelegatedContext context)
        {
            PlannerTask.Status = status;
            PopUpContent = context;
            PopupOpen = true;
        }

        public string StatusDisplayText => 
            PlannerTask.Status switch
            {
                PlannerTaskStatus.Incomplete => "",
                PlannerTaskStatus.Done => "a",
                PlannerTaskStatus.Canceled => "r",
                PlannerTaskStatus.Delegated => "¡",
                PlannerTaskStatus.Pending => "n",
                PlannerTaskStatus.Deferred => "è",
              _ => "Error"    
            };
    
        private static readonly FontFamily Marlett = new FontFamily("Marlett");
        private static readonly FontFamily Wingdings = new FontFamily("Wingdings");

        public FontFamily StatusDisplayFont =>
            PlannerTask.Status switch
            {
                PlannerTaskStatus.Incomplete =>Wingdings,
                PlannerTaskStatus.Delegated =>Wingdings,
                PlannerTaskStatus.Deferred=>Wingdings,
                _ => Marlett
            };

        public static readonly IValueConverter ToolTipOnlyWithText = LambdaConverter.Create(
            (string s)=> string.IsNullOrWhiteSpace(s)?DependencyProperty.UnsetValue:s);

        [AutoNotify] private ITaskPopUpContent popUpContent = new NullContext();
        [AutoNotify] private bool popupOpen;

        public static IValueConverter PriortyBackground = LambdaConverter.Create(
            (char priority) => priority switch
            {
                'A' =>  new SolidColorBrush(Color.FromArgb(127, 255, 0,0)),
                'B' =>  new SolidColorBrush(Color.FromArgb(127, 255, 255,0)),
                'C' =>  new SolidColorBrush(Color.FromArgb(127, 0, 255, 0)),
                'D' =>  new SolidColorBrush(Color.FromArgb(127, 0,0, 255)),
                _ =>  new SolidColorBrush(Color.FromArgb(127, 0, 0,0)),
            });
    }
}