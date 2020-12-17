using Melville.MVVM.Wpf.DiParameterSources;
using Melville.MVVM.Wpf.RootWindows;
using Planner.Models.HtmlGeneration;

namespace Planner.WpfViewModels.PlannerPages
{
    public abstract class PageWithEditNotifications : IAcceptNavigationNotifications
    {
        private readonly IEventBroadcast<NoteEditRequestEventArgs> noteEditRequest;
        
        //This variable has to live here because we want it created inside the window's context so that it picks up
        // the NoteEditRequest EventBroadcast object that is scoped to this window.  The view uses this directly.
        public ILinkRedirect LinkRedirect { get; }
        protected PageWithEditNotifications(IEventBroadcast<NoteEditRequestEventArgs> noteEditRequest, 
            ILinkRedirect linkRedirect)
        {
            this.noteEditRequest = noteEditRequest;
            LinkRedirect = linkRedirect;
        }

        public void NavigatedTo() => noteEditRequest.Fired += DoEditNoteRequest;
        public void NavigatedAwayFrom() => noteEditRequest.Fired -= DoEditNoteRequest;

        protected abstract void DoEditNoteRequest(object? sender, NoteEditRequestEventArgs e);
    }
}