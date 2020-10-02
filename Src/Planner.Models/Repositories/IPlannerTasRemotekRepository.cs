using Planner.Models.Notes;
using Planner.Models.Tasks;

namespace Planner.Models.Repositories
{
    public interface IPlannerTasRemotekRepository: IDatedRemoteRepository<PlannerTask>
    {
    }

    public interface INoteRemoteRepository : IDatedRemoteRepository<Note>
    {
        
    }
}