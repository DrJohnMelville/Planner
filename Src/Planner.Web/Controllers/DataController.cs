using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Planner.Models.Repositories;

namespace Planner.Web.Controllers
{
    public class DataController<TDatum> : Controller 
    {
        private readonly IDatedRemoteRepository<TDatum> source;
        private readonly Func<Guid, TDatum> creator;

        public DataController(IDatedRemoteRepository<TDatum> source, Func<Guid, TDatum> creator)
        {
            this.source = source;
            this.creator = creator;
        }

        [Route("{date}/{timeZone}")]
        [HttpGet]
        public IAsyncEnumerable<TDatum> TasksForDate(LocalDate date, string timeZone) =>
            source.TasksForDate(date, ZoneFromString(timeZone));
        
        private DateTimeZone ZoneFromString (string zoneName) => 
            DateTimeZoneProviders.Tzdb.GetZoneOrNull(HttpUtility.UrlDecode(zoneName)) ??
#if DEBUG
                throw new InvalidOperationException("Cannot find time zone.");
#else
            DateTimeZoneProviders.Tzdb.GetZoneOrNull("America/New_York")??
            DateTimeZone.Utc;
#endif

        [Route("Query")]
        [HttpGet]
        public IAsyncEnumerable<TDatum> TasksForDate([FromBody] Guid[] keys) =>
            source.ItemsFromKeys(keys);

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