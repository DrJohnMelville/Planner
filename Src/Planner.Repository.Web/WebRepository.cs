using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async IAsyncEnumerable<T> TasksForDate(LocalDate date)
        {
            foreach (var task in await webService.Get<T[]>($"{prefix}/{date:yyyy-MM-dd}"))
            {
                yield return task;
            }
        }
    }
}