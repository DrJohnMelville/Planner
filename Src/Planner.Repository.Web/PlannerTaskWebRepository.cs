using Planner.Models.Repositories;
using Planner.Models.Tasks;

namespace Planner.Repository.Web
{
    public class PlannerTasRemoteWebRepository: WebRepository<PlannerTask>, IPlannerTasRemoteRepository
    {
        public PlannerTasRemoteWebRepository(IJsonWebService webService) : base(webService, "/Task")
        {
        }
    }
}