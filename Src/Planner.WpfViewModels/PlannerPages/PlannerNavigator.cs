using System;
using System.ComponentModel.Design.Serialization;
using Melville.MVVM.Time;
using Melville.MVVM.Wpf.RootWindows;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Repositories;
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

        public void ToDate(LocalDate date) => win.NavigateTo(plannerPageFactory(date));
        public void ToEditNote(NoteEditRequestEventArgs args) => win.NavigateTo(editorFactory(args));
    }

    public sealed class ReloadingNavigator : IPlannerNavigator, IDisposable
    {
        private readonly IPlannerNavigator target;
        private readonly IEventBroadcast<ClearCachesEventArgs> reloadEvent;

        public ReloadingNavigator(IPlannerNavigator target, 
            IEventBroadcast<ClearCachesEventArgs> reloadEvent)
        {
            this.target = target;
            this.reloadEvent = reloadEvent;
            reloadEvent.Fired += DoReload;
        }

        public void Dispose() => reloadEvent.Fired -= DoReload;

        private void DoReload(object? sender, ClearCachesEventArgs e) =>
            reloadAction?.Invoke();

        private Action? reloadAction;
        private void DoAction(Action action)
        {
            reloadAction = action;
            action();
        }

        public void ToDate(LocalDate date) => DoAction(()=>target.ToDate(date));

        public void ToEditNote(NoteEditRequestEventArgs args) => 
            DoAction(()=>target.ToEditNote(args));
    }
    
}