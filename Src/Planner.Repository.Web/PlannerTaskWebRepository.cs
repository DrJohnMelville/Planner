using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;
using Planner.Models.Repositories;
using Planner.Models.Tasks;

namespace Planner.Repository.Web
{
    public class PlannerTasRemotekWebRepository: IPlannerTasRemotekRepository
    {
        private readonly IJsonWebService webService;

        public PlannerTasRemotekWebRepository(IJsonWebService webService)
        {
            this.webService = webService;
        }

        public Task Add(PlannerTask task) => webService.Post("/Task", task);
        public Task Update(PlannerTask task) => webService.Put("/Task", task);
        public Task Delete(PlannerTask task) => webService.Delete("/Task/"+task.Key);

        public async IAsyncEnumerable<PlannerTask> TasksForDate(LocalDate date)
        {
            foreach (var task in await webService.Get<PlannerTask[]>($"/Task/{date:yyyy-MM-dd}"))
            {
                yield return task;
            }
        }
    }
}