using System;
using System.Windows.Data;
using Melville.MVVM.Wpf.Bindings;
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

        public static readonly IValueConverter DateConv = LambdaConverter.Create(
            (LocalDate ld)=>ld.ToDateTimeUnspecified(), LocalDate.FromDateTime);
    }
}