using System;
using System.ComponentModel.Design.Serialization;
using Melville.MVVM.Time;
using Melville.MVVM.Wpf.RootWindows;
using NodaTime;
using Planner.Models.Appointments;
using Planner.Models.HtmlGeneration;
using Planner.Models.Repositories;
using Planner.WpfViewModels.Appointments;
using Planner.WpfViewModels.Notes;
using Planner.WpfViewModels.NotesSearchResults;

namespace Planner.WpfViewModels.PlannerPages
{
    public interface IPlannerNavigator
    {
        void ToDate(LocalDate date);
        void ToEditNote(NoteEditRequestEventArgs args);
        void ToNoteSearchPage();
        void ToAppointmentPage(Appointment appointment);
    }

    public class PlannerNavigator : IPlannerNavigator
    {
        private readonly INavigationWindow win;
        private readonly Func<LocalDate, DailyPlannerPageViewModel> plannerPageFactory;
        private readonly Func<NoteEditRequestEventArgs, NoteEditorViewModel> editorFactory;
        private readonly Func<NotesSearchViewModel> notesSearchFactory;
        private readonly Func<Appointment, SingleAppointmentViewModel> appointmentFactory;

        public PlannerNavigator(
            INavigationWindow win, 
            Func<LocalDate, DailyPlannerPageViewModel> plannerPageFactory, 
            Func<NoteEditRequestEventArgs, NoteEditorViewModel> editorFactory,
            Func<NotesSearchViewModel> notesSearchFactory,
            Func<Appointment, SingleAppointmentViewModel> appointmentFactory)
        {
            this.win = win;
            this.plannerPageFactory = plannerPageFactory;
            this.editorFactory = editorFactory;
            this.notesSearchFactory = notesSearchFactory;
            this.appointmentFactory = appointmentFactory;
        }

        public void ToDate(LocalDate date) => win.NavigateTo(plannerPageFactory(date));
        public void ToEditNote(NoteEditRequestEventArgs args) => win.NavigateTo(editorFactory(args));
        public void ToNoteSearchPage() => win.NavigateTo(notesSearchFactory());
        public void ToAppointmentPage(Appointment appointment) => win.NavigateTo(appointmentFactory(appointment));
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

        public void ToNoteSearchPage() =>
            DoAction(() => target.ToNoteSearchPage());

        public void ToAppointmentPage(Appointment appointment) =>
            DoAction(() => target.ToAppointmentPage(appointment));
    }
    
}