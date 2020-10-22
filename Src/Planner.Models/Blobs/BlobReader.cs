using System.IO;
using System.Threading.Tasks;
using NodaTime;

namespace Planner.Models.Blobs
{
    public interface IBlobReader
    {
        Task<Stream> Read(LocalDate date, int ordinal);
    }
    public class BlobReader: IBlobReader
    {
        public Task<Stream> Read(LocalDate date, int ordinal)
        {
            throw new System.NotImplementedException();
        }
    }
}