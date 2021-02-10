using System.IO;
using System.Threading.Tasks;
using System.Windows;
using NodaTime;
using Planner.Models.Blobs;

namespace Planner.Wpf.Notes.Pasters
{
    public abstract class ImageMarkdownPasterBase : IMarkdownPaster
    {
        private string format;
        private string mimeType;
        private IBlobCreator blobCreator;

        protected ImageMarkdownPasterBase(string format, string mimeType, IBlobCreator blobCreator)
        {
            this.format = format;
            this.mimeType = mimeType;
            this.blobCreator = blobCreator;
        }
        public ValueTask<string?> GetPasteText(IDataObject clipboard, LocalDate targetDate) =>
            clipboard.GetDataPresent(format) ? 
                TryGetImage(clipboard, targetDate) : 
                new ValueTask<string?>((string?)null);

        private ValueTask<string?> TryGetImage(IDataObject clipboard, LocalDate targetDate)
        {
            return clipboard.GetData(format) is MemoryStream ms ?
                new ValueTask<string?>(PostImageToServer(targetDate, Convert(ms))) :
                new ValueTask<string?>((string?)null);
        }

        private Task<string?> PostImageToServer(LocalDate targetDate, Stream ms) => 
            blobCreator.MarkdownForNewImage("Pasted Photo", mimeType, targetDate, ms)!;

        protected abstract Stream Convert(Stream clipboardFormat);
        
    }
}