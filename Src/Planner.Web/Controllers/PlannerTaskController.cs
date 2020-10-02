using Microsoft.AspNetCore.Mvc;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Planner.Models.Tasks;

namespace Planner.Web.Controllers
{
    [Route("Task")]
    public class PlannerTaskController : DataController<PlannerTask, IPlannerTasRemotekRepository>
    {
        public PlannerTaskController(IPlannerTasRemotekRepository source) : base(source, g=>new PlannerTask(g))
        {
        }
    }
    [Route("Note")]
    public class NoteController : DataController<Note, INoteRemoteRepository>
    {
        public NoteController(INoteRemoteRepository source) : base(source, g=>new Note(){Key = g})
        {
        }
    }
}