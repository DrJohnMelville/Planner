using System.IO;
using Planner.Models.Blobs;

namespace Planner.WpfViewModels.Notes.Pasters
{
    public class PngMarkdownPaster : ImageMarkdownPasterBase
    {

        public PngMarkdownPaster(IBlobCreator blobCreator): base("PNG", "image/png", blobCreator)
        {
        }

        protected override Stream Convert(Stream clipboardFormat) => clipboardFormat;
    }
}