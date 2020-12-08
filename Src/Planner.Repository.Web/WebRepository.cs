using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using NodaTime;
using Planner.Models.Repositories;

namespace Planner.Repository.Web
{
    public class WebRepository<T>:IDatedRemoteRepository<T> where T:PlannerItemWithDate
    {
        private readonly IJsonWebService webService;
        private readonly string prefix;

        public WebRepository(IJsonWebService webService, string prefix)
        {
            this.webService = webService;
            this.prefix = prefix;
        }

        public Task Add(T task) => webService.Post(prefix, task);
        public Task Update(T task) => webService.Put(prefix, task);
        public Task Delete(T task) => webService.Delete(prefix+"/"+task.Key);
        public async IAsyncEnumerable<T> ItemsFromKeys(IEnumerable<Guid> keys)
        {
            foreach (var task in await webService.Get<Guid[],T[]>($"{prefix}/Query", keys.ToArray()))
            {
                yield return task;
            }
        }

        public async IAsyncEnumerable<T> TasksForDate(LocalDate date, DateTimeZone zone)
        {
            foreach (var task in await webService.Get<T[]>($"{prefix}/{date:yyyy-MM-dd}/{HttpUtility.UrlEncode(zone.Id)}"))
            {
                yield return task;
            }
        }
    }
}