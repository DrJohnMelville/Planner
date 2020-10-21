using System;
using System.IO;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.Repositories;

namespace Planner.Models.Blobs
{
    public interface IBlobCreator
    {
        Task<string> MarkdownForNewImage(string fileName, string mimeType, LocalDate date, Stream data);
    }
    public class BlobCreator: IBlobCreator
    {
        private readonly ILocalRepository<Blob> blobSource;
        private readonly IClock clock;
        private readonly IBlobWriter writer;

        public BlobCreator(ILocalRepository<Blob> blobSource, IClock clock, IBlobWriter writer)
        {
            this.blobSource = blobSource;
            this.clock = clock;
            this.writer = writer;
        }

        public async Task<string> MarkdownForNewImage(
            string fileName, string mimeType, LocalDate date, Stream data)
        {
            var blobList = await blobSource.CompletedItemsForDate(date);
            var record = new Blob()
            {
                Key = Guid.NewGuid(),
                TimeCreated = clock.GetCurrentInstant(),
                Name = fileName,
                MimeType = mimeType
            };
            blobList.Add(record);
            await writer.Write(record, data);
            return $"!({fileName})[{date:M.d}#{blobList.Count}]";
        }
    }
}