using System;
using System.Windows.Input;
using Melville.MVVM.Asyncs;
using Melville.MVVM.Wpf.KeyboardFacade;
using Melville.MVVM.Wpf.RootWindows;
using NodaTime;
using Planner.Models.Appointments;
using Planner.Models.HtmlGeneration;

namespace Planner.Wpf.PlannerPages
{
    public class NewWindowPlannerNavigator : IPlannerNavigator
    {
        private readonly IPlannerNavigator target;
        private readonly Func<(IRootNavigationWindow, IPlannerNavigator)> newWinFactory;
        private readonly IKeyboardQuery keyboard;
        private readonly IStaWorker staWorker;
        public NewWindowPlannerNavigator(IPlannerNavigator target, 
            Func<(IRootNavigationWindow,IPlannerNavigator)> newWinFactory, 
            IKeyboardQuery keyboard, IStaWorker staWorker)
        {
            this.target = target;
            this.newWinFactory = newWinFactory;
            this.keyboard = keyboard;
            this.staWorker = staWorker;
        }

        public IPlannerNavigator Target() =>
            ShouldShoudNavigateInSameWindow() ? 
                target : 
                NavigatorForNewWindows();

        private IPlannerNavigator NavigatorForNewWindows()
        {
            var (newWin, newNav) = newWinFactory();
            newWin.Show();
            return UnwrapCreatedNavigator(newNav);
        }

        private bool ShouldShoudNavigateInSameWindow() => (keyboard.Modifiers & ModifierKeys.Control) == 0;

        private static IPlannerNavigator UnwrapCreatedNavigator(IPlannerNavigator newNav) => 
            ((NewWindowPlannerNavigator)newNav).target;

        public void ToDate(LocalDate date) => staWorker.Run(()=>Target().ToDate(date));

        public void ToEditNote(NoteEditRequestEventArgs args) => 
            staWorker.Run(()=>Target().ToEditNote(args));

        public void ToNoteSearchPage() => staWorker.Run(() => Target().ToNoteSearchPage());
        public void ToAppointmentPage(Appointment appointment) => 
           staWorker.Run(()=>Target().ToAppointmentPage(appointment));
    }
}