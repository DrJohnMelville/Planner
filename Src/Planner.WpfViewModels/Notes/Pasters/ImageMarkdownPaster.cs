using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks.Sources;
using System.Windows.Media.Imaging;
using Markdig.Parsers;
using Melville.MVVM.CSharpHacks;
using Melville.MVVM.Wpf.Clipboards;
using Planner.Models.Blobs;

namespace Planner.WpfViewModels.Notes.Pasters
{
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
            ResetToStartOfStream(ret);
            return ret;
        }

        private static void ResetToStartOfStream(MemoryStream ret) => ret.Seek(0, SeekOrigin.Begin);

        void ConvertToPng(Stream asBitmap, Stream output)
        {
            var reader = new BmpBitmapDecoder(asBitmap, BitmapCreateOptions.None, BitmapCacheOption.None);
            var writer = new PngBitmapEncoder();
            writer.Frames.Add(reader.Frames[0]);
            writer.Save(output);
        }
    }
}