using System.IO;
using System.Windows.Media.Imaging;
using Planner.Models.Blobs;

namespace Planner.Wpf.Notes.Pasters
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