using System.IO;
using System.Threading.Tasks;
using NodaTime;

namespace Planner.Models.Blobs
{
    public interface IBlobCreator
    {
        Task<string> HandleForNewBlob(string fileName, string mimeType, LocalDate date, Stream data);
    }
    public class BlobCreator: IBlobCreator
    {
        public Task<string> HandleForNewBlob(string fileName, string mimeType, LocalDate date, Stream data)
        {
            return Task.FromResult("Blob creation is not yet implemented");
        }
    }
}