using System.Text.RegularExpressions;
using System.Windows;
using Melville.MVVM.Wpf.Clipboards;
using NodaTime;

namespace Planner.WpfViewModels.Notes.Pasters
{
    public class HtmlMarkdownPaster: SynchronousPaster
    {
        public HtmlMarkdownPaster() : base(DataFormats.Html)
        {
        }

        protected override string? ResultFromObject(object? data) => GetFragment(data as string ?? "");

        private static Regex fragmentFinder = new Regex(@"<!--StartFragment-->(.*)<!--EndFragment-->",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private string GetFragment(string text)
        {
            var match = fragmentFinder.Match(text);
            return match.Success ? match.Groups[1].Value : text;
        }
    }
}