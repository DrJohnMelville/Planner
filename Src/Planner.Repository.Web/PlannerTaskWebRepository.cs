using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.Repositories;
using Planner.Models.Tasks;

namespace Planner.Repository.Web
{
    public class PlannerTaskWebRepository: IPlannerTaskRemoteRepository
    {
        private readonly IJsonWebService webService;

        public PlannerTaskWebRepository(IJsonWebService webService)
        {
            this.webService = webService;
        }

        public Task AddTask(PlannerTask task) => webService.Post("/Task", task);
        public Task UpdateTask(PlannerTask task) => webService.Put("/Task", task);
        public Task DeleteTask(PlannerTask task) => webService.Delete("/Task/"+task.Key);

        public async IAsyncEnumerable<PlannerTask> TasksForDate(LocalDate date)
        {
            foreach (var task in await webService.Get<PlannerTask[]>($"/Task/{date:yyyy-MM-dd}"))
            {
                yield return task;
            }
        }
    }
}