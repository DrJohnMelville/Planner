using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Planner.Models.Repositories;
using Planner.Models.Tasks;

namespace Planner.Web.Controllers
{
    [Route("Task")]
    public class PlannerTaskController : Controller
    {
        private readonly IPlannerTaskRepository source;

        public PlannerTaskController(IPlannerTaskRepository source)
        {
            this.source = source;
        }

        [Route("{date}")]
        [HttpGet]
        public IAsyncEnumerable<PlannerTask> TasksForDate(LocalDate date) =>
            source.TasksForDate(date);

        [Route("")]
        [HttpPut]
        public Task Update([FromBody] PlannerTask task) => source.UpdateTask(task);
        [Route("")]
        [HttpPost]
        public Task Add([FromBody] PlannerTask task) => source.AddTask(task);

        [Route("{key}")]
        [HttpDelete]
        public Task DeleteTask(Guid key) =>
            source.DeleteTask(new PlannerTask(key));
    }
}