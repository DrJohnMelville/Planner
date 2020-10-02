﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Planner.Models.Repositories;

namespace Planner.Web.Controllers
{
    public class DataController<TDatum> : Controller 
        where TDatum: PlannerItemWithDate
    {
        private readonly IDatedRemoteRepository<TDatum> source;
        private readonly Func<Guid, TDatum> creator;

        public DataController(IDatedRemoteRepository<TDatum> source, Func<Guid, TDatum> creator)
        {
            this.source = source;
            this.creator = creator;
        }

        [Route("{date}")]
        [HttpGet]
        public IAsyncEnumerable<TDatum> TasksForDate(LocalDate date) =>
            source.TasksForDate(date);

        [Route("")]
        [HttpPut]
        public Task Update([FromBody] TDatum task) => source.Update(task);
        [Route("")]
        [HttpPost]
        public Task Add([FromBody] TDatum task) => source.Add(task);

        [Route("{key}")]
        [HttpDelete]
        public Task DeleteTask(Guid key) =>
            source.Delete(creator(key));
    }
}