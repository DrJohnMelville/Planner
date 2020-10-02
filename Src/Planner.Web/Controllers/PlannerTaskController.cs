using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Planner.Models.Repositories;

namespace Planner.Web.Controllers
{
    [Route("Task")]
    public class PlannerTaskController : Controller
    {
        private readonly IRemotePlannerTaskRepository source;

        public PlannerTaskController(IRemotePlannerTaskRepository source)
        {
            this.source = source;
        }

        [Route("{date}")]
        [HttpGet]
        public IAsyncEnumerable<RemotePlannerTask> TasksForDate(LocalDate date) =>
            source.TasksForDate(date);

        [Route("")]
        [HttpPut]
        public Task Update([FromBody] RemotePlannerTask task) => source.UpdateTask(task);
        [Route("")]
        [HttpPost]
        public Task Add([FromBody] RemotePlannerTask task) => source.AddTask(task);

        [Route("{key}")]
        [HttpDelete]
        public Task DeleteTask(Guid key) =>
            source.DeleteTask(new RemotePlannerTask(key));
    }
}