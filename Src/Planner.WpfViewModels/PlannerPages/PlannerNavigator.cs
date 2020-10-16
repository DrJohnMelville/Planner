using System;
using Melville.MVVM.Wpf.RootWindows;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.WpfViewModels.Notes;

namespace Planner.WpfViewModels.PlannerPages
{
    public interface IPlannerNavigator
    {
        void ToDate(LocalDate date);
        void ToEditNote(NoteEditRequestEventArgs args);
    }

    public class PlannerNavigator : IPlannerNavigator
    {
        private readonly INavigationWindow win;
        private readonly Func<LocalDate, DailyPlannerPageViewModel> plannerPageFactory;
        private readonly Func<NoteEditRequestEventArgs, NoteEditorViewModel> editorFactory;

        public PlannerNavigator(
            INavigationWindow win, 
            Func<LocalDate, DailyPlannerPageViewModel> plannerPageFactory, 
            Func<NoteEditRequestEventArgs, NoteEditorViewModel> editorFactory)
        {
            this.win = win;
            this.plannerPageFactory = plannerPageFactory;
            this.editorFactory = editorFactory;
        }

        public void ToDate(LocalDate date) => InnerNavigateTo(plannerPageFactory(date));
        public void ToEditNote(NoteEditRequestEventArgs args) => win.NavigateTo(editorFactory(args));

        private void InnerNavigateTo(object target)
        {
            // var newWin = newWindowFactory();
            // if (newWin.DataContext is INavigationWindow newNav)
            // {
            //     newWin.Show();
            //     newNav.NavigateTo(target);
            // }
            win.NavigateTo(target);
        }
    }
}