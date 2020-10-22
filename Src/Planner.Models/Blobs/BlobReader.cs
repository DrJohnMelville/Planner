using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.Repositories;

namespace Planner.Models.Blobs
{
    public interface IBlobReader
    {
        Task<Stream> Read(LocalDate date, int ordinal);
    }
    public class BlobReader: IBlobReader
    {
        private readonly ILocalRepository<Blob> repository;
        private readonly IBlobContentStore store;

        public BlobReader(ILocalRepository<Blob> repository, IBlobContentStore store)
        {
            this.repository = repository;
            this.store = store;
        }

        public async Task<Stream> Read(LocalDate date, int ordinal)
        {
            var listForDate = await repository.CompletedItemsForDate(date);
            if (IsInvalidOrdinal(ordinal, listForDate)) return EmptyStream();
            return await store.Read(listForDate[ordinal - 1]);
        }

        private static MemoryStream EmptyStream() => new MemoryStream(Array.Empty<byte>());

        private static bool IsInvalidOrdinal(int ordinal, IList<Blob> listForDate) => 
            ordinal < 1 || ordinal > listForDate.Count;
    }
}