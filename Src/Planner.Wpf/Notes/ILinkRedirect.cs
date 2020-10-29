using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Melville.MVVM.RunShellCommands;
using Planner.Models.HtmlGeneration;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Planner.Models.Time;

namespace Planner.Wpf.Notes
{
    public interface ILinkRedirect
    {
        bool? DoRedirect(string url);
    }
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
                .Select(i => i.Value)
                .DefaultIfEmpty(false)
                .First();
    }

    public class LocalLink : ILinkRedirect
    {
        public bool? DoRedirect(string url) => 
            url.StartsWith("http://localhost:28775") ? (bool?)false : null;
    }
    
    public class EditNotification : ILinkRedirect
    {
        private readonly ILocalRepository<Note> noteRepo;
        private readonly IEventBroadcast<NoteEditRequestEventArgs> notifyEventRequest;

        public EditNotification(ILocalRepository<Note> noteRepo, IEventBroadcast<NoteEditRequestEventArgs> notifyEventRequest)
        {
            this.noteRepo = noteRepo;
            this.notifyEventRequest = notifyEventRequest;
        }

        private static Regex criteria = 
            new Regex(@"(\d{4}-\d{1,2}-\d{1,2})/([0-9a-f]{8}(?:-[0-9a-f]{4}){3}-[0-9a-f]{12})");
        public bool? DoRedirect(string url)
        {
            var match = criteria.Match(url);
            if (!match.Success) return null;

            LaunchEditor(match);
            return true;
        }

        private async void LaunchEditor(Match match)
        {
            if (!(TimeOperations.TryParseLocalDate(match.Groups[1].Value, out var date) &&
                  Guid.TryParse(match.Groups[2].Value, out var noteKey))) return;
            
            var list = await noteRepo.CompletedItemsForDate(date);
            var item = list.FirstOrDefault(i => i.Key == noteKey);
            if (item == null) return;
            
            notifyEventRequest.Fire(this,  new NoteEditRequestEventArgs(list, item));

        }
    }

    public class DefaultToExec : ILinkRedirect
    {
        private readonly IRunShellCommand shell;

        public DefaultToExec(IRunShellCommand shell)
        {
            this.shell = shell;
        }

        public bool? DoRedirect(string url)
        {
            shell.ShellExecute(url);
            return true;
        }
    }
}