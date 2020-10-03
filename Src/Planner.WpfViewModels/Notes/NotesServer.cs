using System.IO;
using System.Net;
using System.Printing;

namespace Planner.WpfViewModels.Notes
{
    public interface INotesServer
    {
        string BaseUrl { get; }
        void Launch();
    }
    public class NotesServer: INotesServer
    {
        public string BaseUrl => "http://localhost:28775/";

        public async void Launch()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add(BaseUrl);
            listener.Start();
            while (true)
            {
                var context = await listener.GetContextAsync();
                using (var writer = new StreamWriter(context.Response.OutputStream))
                {
                    writer.WriteLine("<html><body><h1>Hello World</h1></body></html>");
                }
            }
        }
    }
}