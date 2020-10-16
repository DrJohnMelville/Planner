using System;
using System.Windows.Input;
using Melville.MVVM.Wpf.KeyboardFacade;
using Melville.MVVM.Wpf.RootWindows;
using NodaTime;
using Planner.Models.HtmlGeneration;

namespace Planner.WpfViewModels.PlannerPages
{
    public class NewWindowPlannerNavigator : IPlannerNavigator
    {
        private readonly IPlannerNavigator target;
        private readonly Func<(IRootNavigationWindow, IPlannerNavigator)> newWinFactory;
        private readonly IKeyboardQuery keyboard;
        public NewWindowPlannerNavigator(IPlannerNavigator target, 
            Func<(IRootNavigationWindow,IPlannerNavigator)> newWinFactory, 
            IKeyboardQuery keyboard)
        {
            this.target = target;
            this.newWinFactory = newWinFactory;
            this.keyboard = keyboard;
        }

        public IPlannerNavigator Target() => 
            ShoudNavigateInSameWindow() ? 
                target : 
                NavigatorForNewWindows();

        private IPlannerNavigator NavigatorForNewWindows()
        {
            var (newWin, newNav) = newWinFactory();
            newWin.Show();
            return UnwrapCreatedNavigator(newNav);
        }

        private bool ShoudNavigateInSameWindow() => (keyboard.Modifiers & ModifierKeys.Control) == 0;

        private static IPlannerNavigator UnwrapCreatedNavigator(IPlannerNavigator newNav) => 
            ((NewWindowPlannerNavigator)newNav).target;

        public void ToDate(LocalDate date) => Target().ToDate(date);

        public void ToEditNote(NoteEditRequestEventArgs args) => Target().ToEditNote(args);
    }
}