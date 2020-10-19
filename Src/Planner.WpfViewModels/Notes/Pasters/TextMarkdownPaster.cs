using System.Collections;
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
}