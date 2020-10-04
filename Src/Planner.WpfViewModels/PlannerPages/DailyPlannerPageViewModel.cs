using System;
using Melville.INPC;
using NodaTime;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Planner.Models.Time;
using Planner.WpfViewModels.Notes;
using Planner.WpfViewModels.TaskList;

namespace Planner.WpfViewModels.PlannerPages
{
    public partial class DailyPlannerPageViewModel
    {
        private readonly IClock clock;
        private readonly Func<LocalDate, DailyTaskListViewModel> taskListFactory;
        private readonly INotesServer noteServer;
        public NoteCreator NoteCreator { get; }
        private int nonce = 0;

        [AutoNotify] private LocalDate currentDate;
        [AutoNotify] private DailyTaskListViewModel todayTaskList;
        [AutoNotify] private bool popupOpen;
        public string NotesUrl => $"{noteServer.BaseUrl}{nonce++}/{currentDate:yyyy-M-d}";
        partial void WhenCurrentDateChanges(LocalDate oldValue, LocalDate newValue)
            {
                TodayTaskList = taskListFactory(newValue);
                RefreshNotesUrl();
            }

            private void RefreshNotesUrl() => 
                ((IExternalNotifyPropertyChanged) this).OnPropertyChanged(nameof(NotesUrl));

            public DailyPlannerPageViewModel(
            IClock clock, 
            Func<LocalDate, DailyTaskListViewModel> taskListFactory, 
            INotesServer noteServer,
            NoteCreator noteCreator)
        {
            this.clock = clock;
            this.taskListFactory = taskListFactory;
            this.noteServer = noteServer;
            NoteCreator = noteCreator;
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
    }

    public partial class NoteCreator
    {
        private readonly ILocalRepository<Note> notes;
        private readonly IClock clock;
        [AutoNotify] private string title ="";
        [AutoNotify] private string text ="";

        public NoteCreator(ILocalRepository<Note> notes, IClock clock)
        {
            this.notes = notes;
            this.clock = clock;
        }

        public void Create(LocalDate currentDate)
        {
            if (!ValidNote()) return;
            var newNote = notes.CreateTask(currentDate);
            newNote.Title = title;
            newNote.Text = text;
            newNote.TimeCreated = clock.GetCurrentInstant();

            Title = "";
            Text = "";
        }

        private bool ValidNote() => 
            !(string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Text));
    }
}