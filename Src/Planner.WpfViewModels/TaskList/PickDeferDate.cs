using System;
using NodaTime;

namespace Planner.WpfViewModels.TaskList
{
    public class PickDeferDate: ITaskPopUpContent
    {
        private Action<LocalDate> supplyDate;
        public LocalDate BeginDay { get; }
        public LocalDate SelectedDate { get; set; }
        public object ButtonLabel { get; }
        

        public PickDeferDate(LocalDate beginDay, object buttonLabel, Action<LocalDate> supplyDate)
        {
            this.supplyDate = supplyDate;
            BeginDay = beginDay.PlusDays(1);
            SelectedDate = BeginDay;
            ButtonLabel = buttonLabel;
        }

        public void DoDeferral() => supplyDate(SelectedDate);
    }
}