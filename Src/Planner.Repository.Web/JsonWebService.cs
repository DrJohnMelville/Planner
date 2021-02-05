using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Planner.Repository.Web
{
    public interface IJsonWebService
    {
        public Task<T> Get<T>(string url);
        public Task<T> Get<TBody, T>(string url, TBody body );
        public Task Delete(string url);
        public Task Put<T>(string url, T body);
        public Task Post<T>(string url, T body);
    }

    public class JsonWebService: IJsonWebService
    {
        private HttpClient client;
        private JsonSerializerOptions serializerOptions;

        public JsonWebService(HttpClient client, JsonSerializerOptions serializerOptions)
        {
            this.client = client;
            this.serializerOptions = serializerOptions;
        }
        
    
        public async Task<T> Get<T>( string url) => 
            await ParseGetResponse<T>(await client.GetAsync(url));
        public async Task<T> Get<TBody, T>( string url, TBody body) => 
            await ParseGetResponse<T>(await client.SendAsync(
                new HttpRequestMessage(HttpMethod.Get, url){Content = ObjectAsJsonByteArray(body)}));

        private async Task<T> ParseGetResponse<T>(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            return ObjectFromJsonByteArray<T>(await response.Content.ReadAsByteArrayAsync());
        }


        public Task Delete(string url) => EnsureTaskAsync(client.DeleteAsync(url));
        public Task Put<T>(string url, T body) => EnsureTaskAsync(client.PutAsync(url, ObjectAsJsonByteArray(body)));
        public Task Post<T>(string url, T body) => EnsureTaskAsync(client.PostAsync(url, ObjectAsJsonByteArray(body)));

        private Task EnsureTaskAsync(Task<HttpResponseMessage> input) =>
            input.ContinueWith(i => i.Result.EnsureSuccessStatusCode());
        
        private T ObjectFromJsonByteArray<T>(byte[] text) => 
            JsonSerializer.Deserialize<T>(text, serializerOptions) ??
            throw new InvalidOperationException("Failed to deserialize json to expected object");

        private ByteArrayContent ObjectAsJsonByteArray<T>(T body)
        {
            var ret =  new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(body, serializerOptions));
            SetToJsonContentType<T>(ret);
            return ret;
        }

        private static void SetToJsonContentType<T>(ByteArrayContent ret)
        {
            ret.Headers.Remove("Content-Type");
            ret.Headers.Add("Content-Type", "application/json");
        }
    }
}