using System.Collections.Generic;
using NodaTime;
using Planner.Models.Notes;
using Planner.Models.Repositories;

namespace Planner.WpfViewModels.Notes
{
    public class DailyNotesViewModel
    {
        private readonly IList<Note> notes;
        public IList<Note> Notes => notes;
        private readonly ILocalRepository<Note> noteRepository;
        private readonly LocalDate date;

        public DailyNotesViewModel(ILocalRepository<Note> noteRepository, LocalDate date)
        {
            this.noteRepository = noteRepository;
            this.date = date;
            notes = noteRepository.TasksForDate(date);
        }
    }
}