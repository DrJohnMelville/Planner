using Planner.Models.Notes;
using Planner.Models.Tasks;

namespace Planner.Models.Repositories
{
    public interface IPlannerTasRemoteRepository: IDatedRemoteRepository<PlannerTask>
    {
    }

    public interface INoteRemoteRepository : IDatedRemoteRepository<Note>
    {
        
    }
}