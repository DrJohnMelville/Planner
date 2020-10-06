﻿using System;
using Melville.INPC;
using Melville.MVVM.Wpf.RootWindows;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Time;
using Planner.WpfViewModels.Notes;
using Planner.WpfViewModels.TaskList;

namespace Planner.WpfViewModels.PlannerPages
{
    public partial class DailyPlannerPageViewModel:IAcceptNavigationNotifications
    {
        private readonly Func<LocalDate, DailyTaskListViewModel> taskListFactory;
        private readonly INotesServer noteServer;
        private readonly INavigationWindow navigation;
        private Func<NoteEditRequestEventArgs, NoteEditorViewModel> editorFactory;
        private readonly INoteUrlGenerator urlGen;
        public NoteCreator NoteCreator { get; }

        [AutoNotify] private LocalDate currentDate;
        [AutoNotify] private DailyTaskListViewModel todayTaskList;
        [AutoNotify] private bool popupOpen;
        [AutoNotify] public string NotesUrl => urlGen.DailyUrl(CurrentDate);
        partial void WhenCurrentDateChanges(LocalDate oldValue, LocalDate newValue)
            {
                TodayTaskList = taskListFactory(newValue);
            }

            private void RefreshNotesUrl() => 
                ((IExternalNotifyPropertyChanged) this).OnPropertyChanged(nameof(NotesUrl));

            public DailyPlannerPageViewModel(
            IClock clock, 
            Func<LocalDate, DailyTaskListViewModel> taskListFactory, 
            INotesServer noteServer,
            NoteCreator noteCreator, 
            INavigationWindow navigation, 
            Func<NoteEditRequestEventArgs, NoteEditorViewModel> editorFactory, INoteUrlGenerator urlGen)
        {
            this.taskListFactory = taskListFactory;
            this.noteServer = noteServer;
            NoteCreator = noteCreator;
            this.navigation = navigation;
            this.editorFactory = editorFactory;
            this.urlGen = urlGen;
            currentDate = clock.CurrentDate();
            todayTaskList = taskListFactory(currentDate);
        }

        public void ForwardOneDay() => CurrentDate = CurrentDate.PlusDays(1);
        public void BackOneDay() => CurrentDate = CurrentDate.PlusDays(-1);

        public void CreateNoteOnDay()
        {
            NoteCreator.Create(CurrentDate);
            RefreshNotesUrl();
        }

        public void NavigatedTo() => noteServer.NoteEditRequested += LaunchRequest;
        public void NavigatedAwayFrom() => noteServer.NoteEditRequested -= LaunchRequest;

        private void LaunchRequest(object? sender, NoteEditRequestEventArgs e) => 
            navigation.NavigateTo(editorFactory(e));
    }
}