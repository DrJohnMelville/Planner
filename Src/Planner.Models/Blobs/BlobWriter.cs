using System.IO;
using System.Threading.Tasks;

namespace Planner.Models.Blobs
{
    public interface IBlobWriter
    {
        Task Write(Blob b, Stream target);
    }
    public class BlobWriter: IBlobWriter
    {
        public Task Write(Blob b, Stream target)
        {
            return Task.CompletedTask;
        }
    }
}