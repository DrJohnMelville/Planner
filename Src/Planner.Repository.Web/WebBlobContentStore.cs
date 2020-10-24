using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Planner.Models.Blobs;

namespace Planner.Repository.Web
{
    public class WebBlobContentStore : IBlobContentStore
    {
        private HttpClient client;

        public WebBlobContentStore(HttpClient client)
        {
            this.client = client;
        }
        
        private string Url(Guid key) => $"BlobContent/{key}";

        public Task Write(Guid key, Stream data)
        {
            return client.PutAsync(Url(key), new StreamContent(data));
        }

        public async Task<Stream> Read(Blob blob)
        {
            var resp = await client.GetAsync(Url(blob.Key));
            return await resp.Content.ReadAsStreamAsync();
        }
    }
}