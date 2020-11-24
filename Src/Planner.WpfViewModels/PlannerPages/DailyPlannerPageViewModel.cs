﻿using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CefSharp;
using Melville.INPC;
using Melville.MVVM.Wpf.DiParameterSources;
using Melville.MVVM.Wpf.RootWindows;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Repositories;
using Planner.Models.Tasks;
using Planner.Models.Time;
using Planner.WpfViewModels.TaskList;

namespace Planner.WpfViewModels.PlannerPages
{
    public partial class DailyPlannerPageViewModel:IAcceptNavigationNotifications
    {
        private readonly IPlannerNavigator navigator;
        private readonly INoteUrlGenerator urlGen;
        private readonly IEventBroadcast<NoteEditRequestEventArgs> noteEditRequest;
        public NoteCreator NoteCreator { get; }
        public DailyTaskListViewModel TodayTaskList { get; }
        private readonly LocalDate currentDate;

        //This variable has to live here because we want it created inside the window's context so that it picks up
        // the NoteEditRequest EventBroadcast object that is scoped to this window.  The view uses this directly.
        public IRequestHandler RequestHandler { get; } 

        public LocalDate CurrentDate
        {
            get => currentDate;
            set => navigator.ToDate(value);
        }

        [AutoNotify] private bool popupOpen;
        public string NotesUrl => urlGen.DailyUrl(CurrentDate);

            public DailyPlannerPageViewModel(
            LocalDate currentDate,
            Func<LocalDate, DailyTaskListViewModel> taskListFactory, 
            INotesServer noteServer, // we don't use this, we just need it to exist.  Asking for it forces it to exist.
            NoteCreator noteCreator, 
            IPlannerNavigator navigator, 
            INoteUrlGenerator urlGen, 
            IEventBroadcast<NoteEditRequestEventArgs> noteEditRequest, 
            IRequestHandler requestHandler)
        {
            
            this.navigator = navigator;
            NoteCreator = noteCreator;
            this.urlGen = urlGen;
            this.noteEditRequest = noteEditRequest;
            RequestHandler = requestHandler;
            this.currentDate = currentDate; 
            TodayTaskList = taskListFactory(currentDate);
        }

        public void ForwardOneDay() => navigator.ToDate(CurrentDate.PlusDays(1));
        public void BackOneDay() => navigator.ToDate(CurrentDate.PlusDays(-1));

        public void CreateNoteOnDay()
        {
            NoteCreator.Create(CurrentDate);
            ReloadNotesDisplay();
        }
        //The notes url includes a nonce so we can force updates when the data changes.
        // all we have to do is tell wpf that the url changed and it will read a new
        // value and refresh the webbrowser.
        private void ReloadNotesDisplay() => 
            ((IExternalNotifyPropertyChanged) this).OnPropertyChanged(nameof(NotesUrl));
        
        public void NavigatedTo() => noteEditRequest.Fired += DoEditNoteRequest;
        public void NavigatedAwayFrom() => noteEditRequest.Fired -= DoEditNoteRequest;

        private void DoEditNoteRequest(object? sender, NoteEditRequestEventArgs e) =>
            navigator.ToEditNote(e);

        public void PlannerPageLinkClicked(Segment<TaskTextType> segment)
        {
            if (segment.Match == null) return;
            navigator.NavigateToDate(segment.Match.Groups, CurrentDate);
        }

        public void ReloadCaches([FromServices]IEventBroadcast<ClearCachesEventArgs> signalObject)
        {
            signalObject.Fire(this, new ClearCachesEventArgs());
        }

        public void GoToToday([FromServices] IClock clock)
        {
            navigator.ToDate(clock.CurrentDate());
        }
    }

}