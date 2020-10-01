using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Planner.Models.Repositories;

namespace Planner.Web.Controllers
{
    public class PlannerTaskController : Controller
    {
        private readonly IRemotePlannerTaskRepository source;

        public PlannerTaskController(IRemotePlannerTaskRepository source)
        {
            this.source = source;
        }

        public IAsyncEnumerable<RemotePlannerTask> TasksForDate(LocalDate date) =>
            source.TasksForDate(date);
    }
}