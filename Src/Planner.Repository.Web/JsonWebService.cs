﻿using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Threading.Tasks;

namespace Planner.Repository.Web
{
    public interface IJsonWebService
    {
        public Task<T> Get<T>(string url);
        public Task Delete(string url);
        public Task Put<T>(string url, T body);
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

        public async Task<T> Get<T>( string url)
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return ObjectFromJsonByteArray<T>(await response.Content.ReadAsByteArrayAsync());
        }


        public Task Delete(string url) => client.DeleteAsync(url);
        public Task Put<T>(string url, T body) => client.PutAsync(url, ObjectAsJsonByteArray(body));
        
        private T ObjectFromJsonByteArray<T>(byte[] text) => JsonSerializer.Deserialize<T>(text, serializerOptions);
        private ByteArrayContent ObjectAsJsonByteArray<T>(T body) => new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(body, serializerOptions));

    }
}