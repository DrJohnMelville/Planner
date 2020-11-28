using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Melville.MVVM.RunShellCommands;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Planner.Models.Time;
using Planner.WpfViewModels.PlannerPages;

namespace Planner.Wpf.Notes
{
    public class CompositeLinkRedirect : ILinkRedirect
    {
        private IList<ILinkRedirect> children;

        public CompositeLinkRedirect(IList<ILinkRedirect> children)
        {
            this.children = children;
        }

        public bool? DoRedirect(string url) =>
            children.Select(i => i.DoRedirect(url))
                .Where(i => i.HasValue)
                .Select(i => i!.Value)
                .DefaultIfEmpty(false)
                .First();
    }
    
    public abstract class RegexLink: ILinkRedirect
    {
        private Regex pattern;
        protected RegexLink(Regex pattern) => this.pattern = pattern;

        public bool? DoRedirect(string url)
        {
            var match = pattern.Match(url);
            return match.Success ? DoRedirect(match) : null;
        }

        protected abstract bool? DoRedirect(Match match);
    }

    
    public class EditNotification :RegexLink
    {
        private readonly ILocalRepository<Note> noteRepo;
        private readonly IEventBroadcast<NoteEditRequestEventArgs> notifyEventRequest;

        public EditNotification(ILocalRepository<Note> noteRepo, IEventBroadcast<NoteEditRequestEventArgs> notifyEventRequest):
            base(new Regex(@"(\d{4}-\d{1,2}-\d{1,2})/([0-9a-f]{8}(?:-[0-9a-f]{4}){3}-[0-9a-f]{12})"))
        {
            this.noteRepo = noteRepo;
            this.notifyEventRequest = notifyEventRequest;
        }

        protected override bool? DoRedirect(Match match)
        {
            LaunchEditor(match);
            return true;
        }

        private async void LaunchEditor(Match match)
        {
            if (!(TimeOperations.TryParseLocalDate(match.Groups[1].Value, out var date) &&
                  Guid.TryParse(match.Groups[2].Value, out var noteKey))) return;
            
            var list = await noteRepo.ItemsForDate(date).CompleteList();
            var item = list.FirstOrDefault(i => i.Key == noteKey);
            if (item == null) return;
            notifyEventRequest.Fire(this,  new NoteEditRequestEventArgs(list, item));
        }
    }
    public class PlannerNavigateNotification :RegexLink
    {
        private readonly IPlannerNavigator navigator;
        public PlannerNavigateNotification(IPlannerNavigator navigator):
            base(new Regex(@"/navToPage/(\d{4}-\d{1,2}-\d{1,2})"))
        {
            this.navigator = navigator;
        }

        protected override bool? DoRedirect(Match match)
        {
            if (TimeOperations.TryParseLocalDate(match.Groups[1].Value, out var date))
            {
                navigator.ToDate(date);
            }
            return true;
        }
    }

    public class OpenLocalFile : RegexLink
    {
        private readonly IRunShellCommand runShellCommand;

        public OpenLocalFile(IRunShellCommand runShellCommand):base(new Regex(@"/LocalFile/(.+)$"))
        {
            this.runShellCommand = runShellCommand;
        }

        protected override bool? DoRedirect(Match match)
        {
            runShellCommand.ShellExecute(HttpUtility.UrlDecode(match.Groups[1].Value));
            return true;
        }
    }

    public class RunNonPlannerUrlsInSystemBrowser : ILinkRedirect
    {
        private readonly IRunShellCommand shell;

        public RunNonPlannerUrlsInSystemBrowser(IRunShellCommand shell)
        {
            this.shell = shell;
        }

        public bool? DoRedirect(string url)
        {
            if (url.StartsWith("http://localhost:28775")) return null;
            shell.ShellExecute(url);
            return true;
        }
    }
}