using System;
using System.IO;
using System.Threading.Tasks;
using Melville.MVVM.FileSystem;

namespace Planner.Models.Blobs
{
    public interface IBlobContentStore
    {
        Task Write(Guid key, Stream data);
        Task Write(Blob blob, Stream data) => this.Write(blob.Key, data);
        Task<Stream> Read(Blob blob); 
    }

    public interface IDeletableBlobContentStore : IBlobContentStore
    {
        void Delete(Blob blob);
    } 
    public class BlobContentContentStore: IDeletableBlobContentStore
    {
        private readonly IDirectory targetDir;
        public BlobContentContentStore(IDirectory targetDir)
        {
            this.targetDir = targetDir;
        }
        private IFile FileFromKey(Guid key) => targetDir.File(key.ToString());
        public async Task Write(Guid key, Stream data)
        {
            await using var destination = await FileFromKey(key).CreateWrite();
            await data.CopyToAsync(destination);
        }
        public Task<Stream> Read(Blob blob) => FileFromKey(blob.Key).OpenRead();
        public void Delete(Blob blob) => FileFromKey(blob.Key).Delete();
    }
}