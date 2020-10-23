using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Repositories;
using Planner.Models.Time;

namespace Planner.Models.Blobs
{
    public class BlobContentOption: HtmlContentOption
    {
        private readonly ILocalRepository<Blob> repository;
        private readonly IBlobContentStore store;

        public BlobContentOption(ILocalRepository<Blob> repository, IBlobContentStore store):
            base(new Regex(@"([\d-]+)\/(\d+)\.(\d+)(?:\.(\d+))?_(\d+)"))
        {
            this.repository = repository;
            this.store = store;
        }

        protected override Task? TryRespond(Match match, Stream destination)
        {
            if (!TimeOperations.TryParseLocalDate(match.Groups[1].Value, out var pageDate)) return null;

            return CopyBlob(
                ConstructDateFromMatch(pageDate, match.Groups), 
                ExtractOrdinalFromMatch(match), destination);
        }
        
        private static int ExtractOrdinalFromMatch(Match match)
        {
            return int.Parse(match.Groups[5].Value);
        }

        private static LocalDate ConstructDateFromMatch(LocalDate pageDate, GroupCollection groups) =>
            ContextualDateParser.SelectedDate(groups[4].Value, groups[2].Value, groups[3].Value,
                pageDate);

        private async Task CopyBlob(LocalDate date, int ordinal, Stream destination)
        {
            var listForDate = await repository.CompletedItemsForDate(date);
            await using var data = await GetStreamFromOrdinal(ordinal, listForDate);
            await data.CopyToAsync(destination);
        }

        private Task<Stream> GetStreamFromOrdinal(int ordinal, IList<Blob> listForDate)
        {
            return IsInvalidOrdinal(ordinal, listForDate) ? 
                EmptyStream() : 
                store.Read(listForDate[ordinal - 1]);
        }


        private static Task<Stream> EmptyStream() => 
            Task.FromResult((Stream)new MemoryStream(Array.Empty<byte>()));

        private static bool IsInvalidOrdinal(int ordinal, IList<Blob> listForDate) => 
            ordinal < 1 || ordinal > listForDate.Count;
    }
}