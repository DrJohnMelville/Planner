using CefSharp;
using CefSharp.Handler;

namespace Planner.Wpf.Notes
{
    public class BlockAllNavigationHandler : RequestHandler
    {
        protected override bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture,
            bool isRedirect)
        {
            return false;
        }
    }
}