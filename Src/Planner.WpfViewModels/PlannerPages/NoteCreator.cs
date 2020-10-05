using Melville.INPC;
using NodaTime;
using Planner.Models.Notes;
using Planner.Models.Repositories;

namespace Planner.WpfViewModels.PlannerPages
{
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
            notes.CreateItem(currentDate, newNote =>
            {
                newNote.Title = title;
                newNote.Text = text;
                newNote.TimeCreated = clock.GetCurrentInstant();
            });

            Title = "";
            Text = "";
        }

        private bool ValidNote() => 
            !(string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Text));
    }
}