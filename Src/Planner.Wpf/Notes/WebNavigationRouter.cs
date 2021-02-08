using System.Windows.Data;
using CefSharp;
using CefSharp.Handler;
using Melville.MVVM.Wpf.Bindings;
using Planner.Wpf.PlannerPages;

namespace Planner.Wpf.Notes
{
    public class WebNavigationRouter:RequestHandler
    {
        private readonly ILinkRedirect redirect;

        public WebNavigationRouter(ILinkRedirect redirect)
        {
            this.redirect = redirect;
        }

        protected override bool OnBeforeBrowse(
            IWebBrowser chromiumWebBrowser, IBrowser browser, 
            IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            if ((!userGesture) || isRedirect) return false;
            return redirect.DoRedirect(request.Url) ?? false;
        }

        public static readonly IValueConverter FromLinkRedirect = LambdaConverter.Create(
            (ILinkRedirect ld) => new WebNavigationRouter(ld));
    }

}