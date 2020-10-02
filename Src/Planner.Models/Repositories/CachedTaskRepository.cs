using Planner.Models.Tasks;

namespace Planner.Models.Repositories
{
    public class CachedTaskRepository:CachedRepository<PlannerTask>, ILocalPlannerTaskRepository
    {
        public CachedTaskRepository(ILocalPlannerTaskRepository source): base (source)
        {
        }
    }
}