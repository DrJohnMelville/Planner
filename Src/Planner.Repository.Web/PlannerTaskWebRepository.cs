using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.Repositories;

namespace Planner.Repository.Web
{
    public class PlannerTaskWebRepository: IRemotePlannerTaskRepository
    {
        private readonly IJsonWebService webService;

        public PlannerTaskWebRepository(IJsonWebService webService)
        {
            this.webService = webService;
        }

        public Task AddOrUpdateTask(RemotePlannerTask task) => throw new System.NotImplementedException();

        public Task DeleteTask(RemotePlannerTask task) => throw new System.NotImplementedException();

        public async IAsyncEnumerable<RemotePlannerTask> TasksForDate(LocalDate date)
        {
            foreach (var task in await webService.Get<RemotePlannerTask[]>($"/Task/{date:yyyy-MM-dd}"))
            {
                yield return task;
            }
        }
    }
}