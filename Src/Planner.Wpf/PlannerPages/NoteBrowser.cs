using System.Windows;
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
    }
}