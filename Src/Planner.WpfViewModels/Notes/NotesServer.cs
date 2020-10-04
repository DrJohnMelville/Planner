using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Printing;
using System.Security.RightsManagement;
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
        public NotesServer(INoteHtmlGenerator generator)
        {
            this.generator = generator;
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

        private static StreamWriter CreateOutputWriter(HttpListenerContext context) => 
            new StreamWriter(context.Response.OutputStream);

        private static Task<HttpListenerContext> WaitForNextRequest(HttpListener listener) => 
            listener.GetContextAsync();

        private HttpListener SetupHttpListener()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add(BaseUrl);
            listener.Start();
            return listener;
        }

        private string TrimPrefixFromUrl(HttpListenerContext context) => context.Request.Url.ToString()[BaseUrl.Length..];
    }
}