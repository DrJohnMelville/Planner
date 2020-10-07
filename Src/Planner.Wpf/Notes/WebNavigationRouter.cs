using CefSharp;
using CefSharp.Handler;
using Melville.MVVM.RunShellCommands;

namespace Planner.Wpf.Notes
{
    public class WebNavigationRouter:RequestHandler
    {
        private readonly IRunShellCommand runner;

        public WebNavigationRouter(IRunShellCommand runner)
        {
            this.runner = runner;
        }

        protected override bool OnBeforeBrowse(
            IWebBrowser chromiumWebBrowser, IBrowser browser, 
            IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            if ((!userGesture) || isRedirect) return false;
            if (request.Url.StartsWith("http://localhost:28775")) return false;
            runner.ShellExecute(request.Url);
            return true;
        }
    }
}