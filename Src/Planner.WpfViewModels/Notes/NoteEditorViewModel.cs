using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Reflection.Metadata;
using System.Transactions;
using Melville.INPC;
using Melville.MVVM.Wpf.RootWindows;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Notes;
using Planner.WpfViewModels.PlannerPages;

namespace Planner.WpfViewModels.Notes
{
    [AutoNotify]
    public partial class NoteEditorViewModel: IExternalNotifyPropertyChanged
    {
        public Note Note { get; }
        private readonly IList<Note> notesForDay; 
        private INoteUrlGenerator urlGen;
        private INavigationWindow navigator;
        private Func<LocalDate, DailyPlannerPageViewModel> plannerPageFactory;
        private readonly Action cancelOperation;
        
        public NoteEditorViewModel(
            NoteEditRequestEventArgs request, 
            INoteUrlGenerator urlGen, 
            INavigationWindow navigator, 
            Func<LocalDate, DailyPlannerPageViewModel> plannerPageFactory)
        {
            this.urlGen = urlGen;
            this.navigator = navigator;
            this.plannerPageFactory = plannerPageFactory;
            Note = request.Note;
            notesForDay = request.DailyList;
            cancelOperation = CreateCancelOperation(Note.Title, Note.Text);
            Note.PropertyChanged += UpdateWhenNoteChanges;
        }
        
        public void UpdateWhenNoteChanges(object? sender, EventArgs e)=>
            ((IExternalNotifyPropertyChanged) this).OnPropertyChanged(nameof(NoteUrl));
        
        private Action CreateCancelOperation(string title, string text) => () =>
        {
            Note.Title = title;
            Note.Text = text;
        };

        public string DisplayCreationDate => 
            $@"Note Created: {Note.TimeCreated
                .InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault()).LocalDateTime:f}";

        public string NoteUrl => urlGen.EditNoteUrl(Note);


        public void NavigateToPlannerPage() => LeavePage(false);
        public void CancelEdit() => LeavePage(true);
        private void LeavePage(bool shouldRevertNote)
        {
            UnhookNoteUpdates();
            if (shouldRevertNote) cancelOperation();
            navigator.NavigateTo(plannerPageFactory(Note.Date));
        }
        private void UnhookNoteUpdates() => Note.PropertyChanged -= UpdateWhenNoteChanges;
    }
}