using System.Collections;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Markdig.Parsers;
using Melville.MVVM.Wpf.Clipboards;
using NodaTime;
using Planner.Models.Blobs;

namespace Planner.WpfViewModels.Notes.Pasters
{
    public class PngMarkdownPaster : IMarkdownPaster
    {
        private readonly IBlobCreator blobCreator;

        public PngMarkdownPaster(IBlobCreator blobCreator)
        {
            this.blobCreator = blobCreator;
        }

        public ValueTask<string?> GetPasteText(IDataObject clipboard, LocalDate targetDate)
        {
            if (!clipboard.GetDataPresent("PNG")) return new ValueTask<string?>((string?)null);
            return
                clipboard.GetData("PNG") is MemoryStream ms ?
                    new ValueTask<string?>(PostImageToServer(targetDate, ms)) :
                    new ValueTask<string?>((string?)null);
        }

        private Task<string?> PostImageToServer(LocalDate targetDate, MemoryStream ms) => 
            blobCreator.HandleForNewBlob("Pasted Photo", "image/png", targetDate, ms)!;
    }
}