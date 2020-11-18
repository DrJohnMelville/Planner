using System.Windows;
using CefSharp;
using CefSharp.Wpf;

namespace Planner.Wpf.PlannerPages
{
    public class NoteBrowser:ChromiumWebBrowser
    {
        public static readonly DependencyProperty IsNavigatingProperty = DependencyProperty.Register("IsNavigating",
            typeof(bool), typeof(NoteBrowser), new FrameworkPropertyMetadata(false));

        public bool IsNagivating
        {
            get => (bool) GetValue(IsNavigatingProperty);
            set => SetValue(IsNavigatingProperty, value);
        }
        public NoteBrowser()
        {
            this.LoadingStateChanged += (s, e) =>
            {
                Dispatcher.Invoke(() => IsNagivating = e.IsLoading);
            };
        }
        
        public static readonly DependencyProperty BoundRequestHandlerProperty = 
            DependencyProperty.Register("BoundRequestHandler",
            typeof(IRequestHandler), typeof(NoteBrowser), new FrameworkPropertyMetadata(null, BRHChanged));

        private static void BRHChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NoteBrowser nb) nb.RequestHandler = (IRequestHandler) e.NewValue;
        }

        public IRequestHandler BoundRequestHandler
        {
            get => (IRequestHandler) GetValue(BoundRequestHandlerProperty);
            set => SetValue(BoundRequestHandlerProperty, value);
        }
        
    }
}