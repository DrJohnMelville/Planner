using System.Collections;
using System.Text.RegularExpressions;
using System.Windows;
using Melville.MVVM.Wpf.Clipboards;
using NodaTime;

namespace Planner.WpfViewModels.Notes.Pasters
{
    public class TextMarkdownPaster: IMarkdownPaster
    {
        private readonly IReadFromClipboard clipboard;

        public TextMarkdownPaster(IReadFromClipboard clipboard)
        {
            this.clipboard = clipboard;
        }

        public string? GetPasteText(LocalDate targetDate)
        {
            if (clipboard.ContainsText(TextDataFormat.UnicodeText))
            {
                return clipboard.GetText(TextDataFormat.UnicodeText);
            }

            return null;
        }
    }
    
    public class HtmlMarkdownPaster: IMarkdownPaster
    {
        private readonly IReadFromClipboard clipboard;

        public HtmlMarkdownPaster(IReadFromClipboard clipboard)
        {
            this.clipboard = clipboard;
        }

        public string? GetPasteText(LocalDate targetDate)
        {
            if (clipboard.ContainsText(TextDataFormat.Html))
            {
                return GetFragment(clipboard.GetText(TextDataFormat.Html));
            }

            return null;
        }

        private static Regex fragmentFinder = new Regex(@"<!--StartFragment-->(.*)<!--EndFragment-->",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private string GetFragment(string text)
        {
            var match = fragmentFinder.Match(text);
            return match.Success ? match.Groups[1].Value : text;
        }
    }
    
    
}