using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Text;
using Melville.INPC;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Notes;
using Planner.Models.Repositories;

namespace Planner.WpfViewModels.Notes
{
    [AutoNotify]
    public partial class NoteEditorViewModel: IExternalNotifyPropertyChanged
    {
        public Note Note { get; }
        private readonly IList<Note> notesForDay; 
        private INoteUrlGenerator urlGen;
        public NoteEditorViewModel(NoteEditRequestEventArgs request, INoteUrlGenerator urlGen)
        {
            this.urlGen = urlGen;
            Note = request.Note;
            notesForDay = request.DailyList;
            Note.PropertyChanged += (s, e) =>
                ((IExternalNotifyPropertyChanged) this).OnPropertyChanged(nameof(NoteUrl));
        }

        public string DisplayCreationDate => 
            $@"Note Created: {Note.TimeCreated
                .InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault()).LocalDateTime:f}";

        public string NoteUrl => urlGen.EditNoteUrl(Note);
    }
}