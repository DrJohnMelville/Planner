using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.Repositories;

namespace Planner.Models.Blobs
{
    public class CompopsiteBlobRemoteRepository : IDatedRemoteRepository<Blob>
    {
        private readonly IDatedRemoteRepository<Blob> inner;
        private readonly IDeletableBlobContentStore store;

        public CompopsiteBlobRemoteRepository(IDatedRemoteRepository<Blob> inner, IDeletableBlobContentStore store)
        {
            this.inner = inner;
            this.store = store;
        }

        public Task Add(Blob task) => inner.Add(task);
        public Task Update(Blob task) => inner.Update(task);
        public IAsyncEnumerable<Blob> TasksForDate(LocalDate date) => inner.TasksForDate(date);
        public IAsyncEnumerable<Blob> ItemsFromKeys(IEnumerable<Guid> keys) => inner.ItemsFromKeys(keys);

        public Task Delete(Blob task)
        {
            store.Delete(task);
            return inner.Delete(task);
        }

    }
}