using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Repositories;
using Planner.Models.Time;

namespace Planner.Models.Blobs
{
    public class BlobStreamExtractor
    {
        private readonly ILocalRepository<Blob> repository;
        private readonly IBlobContentStore store;

        public BlobStreamExtractor(ILocalRepository<Blob> repository, IBlobContentStore store)
        {
            this.repository = repository;
            this.store = store;
        }

        public async Task<(Stream Stream, string MimeType)> FromGuid(Guid guid) => 
            await Output(await LookupByGuid(guid));

        public async Task<(Stream, string MimeType)> FromComponents(
            LocalDate pageDate, string year, string month, string day, string ordinal) =>
            await Output( await LookupByComponents(pageDate, year, month, day, ordinal));


        private async Task<Blob?> LookupByGuid(Guid guid) =>
            (await repository.ItemsByKeys(new[] {guid}).CompleteList()).FirstOrDefault();

        private async Task<(Stream Stream, string MimeType)> Output(Blob? blob) =>
            blob == null? 
                (new MemoryStream(Array.Empty<byte>()), ""):
                (await store.Read(blob), blob.MimeType);
        
        private async Task<Blob?> LookupByComponents(
            LocalDate pageDate, string year, string month, string day, string ordinal)
        {
            var itemDate = ContextualDateParser.SelectedDate(year, month, day, pageDate);
            var listForDate = await repository.ItemsForDate(itemDate).CompleteList();
            var intOrdinal = int.Parse(ordinal);
            return (intOrdinal > 0 && intOrdinal <= listForDate.Count) ? listForDate[intOrdinal - 1] : null;
        }
    }
    public class BlobGenerator: TryNoteHtmlGenerator
    {
        private readonly BlobStreamExtractor extractor;
        private static readonly Regex filter = new(
            @"\/Images\/(\d{4}-\d{1,2}-\d{1,2})\/(\d+)\.(\d+)(?:\.(\d+))?_(\d+)", RegexOptions.IgnoreCase);

        public BlobGenerator(BlobStreamExtractor extractor): base(filter)
        {
            this.extractor = extractor;
        }

        protected override Task? TryRespond(Match match, Stream destination)
        {
            if (!TimeOperations.TryParseLocalDate(match.Groups[1].Value, out var pageDate)) return null;
            GroupCollection groups = match.Groups;
            return CopyBlobForParsedRequest(
                destination, pageDate, groups[4].Value, groups[2].Value, groups[3].Value, match.Groups[5].Value);
        }
        
        private async Task CopyBlobForParsedRequest(
            Stream destination, LocalDate pageDate, string year, string month, string day, string ordinal)
        {
            var (stream, mime) = await extractor.FromComponents(pageDate, year, month, day, ordinal);
            await using (stream)
            {
                await stream.CopyToAsync(destination);
            }
        }
    }
}