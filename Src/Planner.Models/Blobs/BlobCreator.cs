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
        private readonly IBlobContentStore contentStore;

        public BlobCreator(ILocalRepository<Blob> blobSource, IClock clock, IBlobContentStore contentStore)
        {
            this.blobSource = blobSource;
            this.clock = clock;
            this.contentStore = contentStore;
        }

        public async Task<string> MarkdownForNewImage(
            string fileName, string mimeType, LocalDate date, Stream data)
        {
            var blobList = await blobSource.CompletedItemsForDate(date);
            var record = blobSource.CreateItem(date, i=>
            {
                i.Key = Guid.NewGuid();
                i.TimeCreated = clock.GetCurrentInstant();
                i.Name = fileName;
                i.MimeType = mimeType;
            });
            await contentStore.Write(record, data);
            return $"![{fileName}]({date:M.d}_{blobList.Count})";
        }
    }
}