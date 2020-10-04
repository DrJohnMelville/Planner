using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Printing;
using System.Security.RightsManagement;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Planner.Models.HtmlGeneration;
using Planner.Models.Notes;

namespace Planner.WpfViewModels.Notes
{
    public interface INotesServer
    {
        string BaseUrl { get; }
        void Launch();
    }
    public class NotesServer: INotesServer
    {
        private readonly INoteHtmlGenerator generator;
        private Regex extractQuery;
        public NotesServer(INoteHtmlGenerator generator)
        {
            this.generator = generator;
            extractQuery = new Regex($"{Regex.Escape(BaseUrl)}\\d+/(.*)");
        }

        public string BaseUrl => "http://localhost:28775/";
        
        public async void Launch()
        {
            var listener = SetupHttpListener();
            while (true)
            {
                var context = await WaitForNextRequest(listener);
                await using (var writer = CreateOutputWriter(context))
                {
                    await generator.GenerateResponse(TrimPrefixFromUrl(context), writer);
                }
            }
        }

        private static Stream CreateOutputWriter(HttpListenerContext context) => 
            context.Response.OutputStream;

        private static Task<HttpListenerContext> WaitForNextRequest(HttpListener listener) => 
            listener.GetContextAsync();

        private HttpListener SetupHttpListener()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add(BaseUrl);
            listener.Start();
            return listener;
        }

        private string TrimPrefixFromUrl(HttpListenerContext context)
        {
            var uriString = context.Request.Url.OriginalString;
            var match = extractQuery.Match(uriString);
            return match.Success?match.Groups[1].Value:uriString;
        }
    }
}