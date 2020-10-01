using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Threading.Tasks;

namespace Planner.Repository.Web
{
    public interface IJsonWebService
    {
        public Task<T> Get<T>(string url);
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
            var text = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(text, serializerOptions);
        }
    }
}