using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using System.Windows;
using System.Windows.Media.Imaging;
using Markdig.Parsers;
using Melville.MVVM.CSharpHacks;
using Melville.MVVM.Wpf.Clipboards;
using NodaTime;
using Planner.Models.Blobs;

namespace Planner.WpfViewModels.Notes.Pasters
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
    public class PngMarkdownPaster : ImageMarkdownPasterBase
    {

        public PngMarkdownPaster(IBlobCreator blobCreator): base("PNG", "image/png", blobCreator)
        {
        }

        protected override Stream Convert(Stream clipboardFormat) => clipboardFormat;
    }

    public class ImageMarkdownPaster : ImageMarkdownPasterBase
    {
        public ImageMarkdownPaster(IBlobCreator blobCreator): base("DeviceIndependentBitmap", 
            "image/png", blobCreator)
        {
        }

        protected override Stream Convert(Stream clipboardFormat)
        {
            var ret = new MemoryStream();
            ConvertToPng(new BitmapPrefixStream(clipboardFormat), ret);
            ret.Seek(0, SeekOrigin.Begin);
            return ret;
        }
        void ConvertToPng(Stream asBitmap, Stream output)
        {
            var reader = new BmpBitmapDecoder(asBitmap, BitmapCreateOptions.None, BitmapCacheOption.None);
            var writer = new PngBitmapEncoder();
            writer.Frames.Add(reader.Frames[0]);
            writer.Save(output);
        }
    }
}