using System;
using System.Windows;
using Melville.DependencyPropertyGeneration;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace Planner.Wpf.PlannerPages
{
    public static partial class WebView2Adapter
    {
        [GenerateDP()]
        private static void OnBoundSourceChanged(DependencyObject wv, string url) =>
            ((WebView2)wv).Source = new Uri(url, UriKind.Absolute);

        [GenerateDP(typeof(bool), "IsNavigating", Attached = true)]
        [GenerateDP]
        private static void OnLinkRedirectChanged(DependencyObject target, ILinkRedirect ld)
        {
            if (target is WebView2 wv)
            {
                wv.NavigationStarting += OnNavigationStarting;
                wv.NavigationCompleted += OnNavigationCompleted;
            }
        }

        private static void OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            if (sender is not WebView2 wv2) return;
            if (GetLinkRedirect(wv2).DoRedirect(e.Uri) ?? false)
            {
                e.Cancel = true;
                return;
            }
            SetIsNavigating(wv2, true);
        }

        private static void OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (sender is WebView2 wv) SetIsNavigating(wv, false);
        }
    }
}