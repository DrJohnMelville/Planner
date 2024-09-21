using Melville.INPC;
using Melville.MVVM.Wpf.DiParameterSources;
using Melville.MVVM.Wpf.ViewFrames;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Wpf.PlannerPages;
using NoteCreator = Planner.Wpf.PlannerPages.NoteCreator;

namespace Planner.Wpf.Notes
{
    [OnDisplayed(nameof(RequireNoteServerToExist))]
    [AutoNotify]
    public partial class DailyNoteDisplayViewModel
    {
        private readonly INoteUrlGenerator urlGen;
        public NoteCreator NoteCreator { get; }
        public ILinkRedirect LinkRedirect { get; }
        private readonly LocalDate currentDate;
        public string NotesUrl => urlGen.DailyUrl(currentDate);

        [AutoNotify] private bool isNavigating;

        public DailyNoteDisplayViewModel(
            INoteUrlGenerator urlGen,
            LocalDate currentDate, 
            NoteCreator noteCreator, 
            ILinkRedirect linkRedirect)
        {
            this.urlGen = urlGen;
            this.currentDate = currentDate;
            NoteCreator = noteCreator;
            LinkRedirect = linkRedirect;  // scoped so I get the same as my parent
        }

        public void RequireNoteServerToExist([FromServices] NotesServer _)
        {
            // Notes Server is singleton, so just depending on it means that the object exist.
        }
        
        public void CreateNoteOnDay()
        {
            NoteCreator.Create(currentDate);
            ReloadNotesDisplay();
        }
        //The notes url includes a nonce so we can force updates when the data changes.
        // all we have to do is tell wpf that the url changed and it will read a new
        // value and refresh the webbrowser.
        private void ReloadNotesDisplay() => 
            ((IExternalNotifyPropertyChanged) this).OnPropertyChanged(nameof(NotesUrl));
    }
}