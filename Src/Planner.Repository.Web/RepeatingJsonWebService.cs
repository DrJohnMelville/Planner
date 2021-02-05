using System;
using System.Threading.Tasks;

namespace Planner.Repository.Web
{
    public class RepeatingJsonWebService : IJsonWebService
    {
        private IJsonWebService inner;

        public RepeatingJsonWebService(IJsonWebService inner)
        {
            this.inner = inner;
        }

        private async Task<T> Try10Times<T>(Func<Task<T>> method)
        {
            for (int i = 0; true; i++)
            {
                try
                {
                    return await method();
                }
                catch (Exception)
                {
                    if (i >= 9) throw;
                    // just catch and retry but rethrow the last one because we have nothing to return
                }
            }
        }
        private async Task Try10Times(Func<Task> method)
        {
            for (int i = 0; true; i++)
            {
                try
                {
                    await method();
                }
                catch (Exception)
                {
                    if (i >= 9) throw;
                    // just catch and retry but rethrow the last one because we have nothing to return
                }
            }
        }

        public Task<T> Get<T>(string url) => Try10Times(() => inner.Get<T>(url));

        public Task<T> Get<TBody, T>(string url, TBody body) => Try10Times(()=>inner.Get<TBody, T>(url, body));

        public Task Delete(string url) => Try10Times(() => inner.Delete(url));

        public Task Put<T>(string url, T body) => Try10Times(() => inner.Put(url, body));

        public Task Post<T>(string url, T body) => Try10Times(() => inner.Post(url, body));
    }
}