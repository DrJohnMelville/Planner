using System.IO;
using System.Threading.Tasks;
using Melville.MVVM.FileSystem;

namespace Planner.Models.Blobs
{
    public interface IBlobContentStore
    {
        Task Write(Blob blob, Stream data);
        Task<Stream> Read(Blob blob);
    }
    public class BlobContentStore: IBlobContentStore
    {
        private readonly IDirectory targetDir;
        public BlobContentStore(IDirectory targetDir)
        {
            this.targetDir = targetDir;
        }
        private IFile FileFromBlob(Blob blob) => targetDir.File(blob.Key.ToString());
        public async Task Write(Blob blob, Stream data)
        {
            await using var destination = await FileFromBlob(blob).CreateWrite();
            await data.CopyToAsync(destination);
        }
        public Task<Stream> Read(Blob blob) => FileFromBlob(blob).OpenRead();
    }
}