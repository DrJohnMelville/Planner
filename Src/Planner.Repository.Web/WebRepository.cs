using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using NodaTime;
using Planner.Models.Appointments;
using Planner.Models.Repositories;

namespace Planner.Repository.Web
{
    public class WebRepository<T> : WebRepositoryBase<T> where T : PlannerItemWithDate
    {
        public WebRepository(IJsonWebService webService, string prefix) : base(webService, prefix)
        {
        }

        protected override string KeyString(T item) => item.Key.ToString();
    }

    public class AppointmentRepository : WebRepositoryBase<Appointment>
    {
        public AppointmentRepository(IJsonWebService webService, string prefix) : base(webService, prefix)
        {
        }

        protected override string KeyString(Appointment item) => 
            item.AppointmentDetailsId.ToString();
    }
    public abstract class WebRepositoryBase<T>:IDatedRemoteRepository<T> 
    {
        private readonly IJsonWebService webService;
        private readonly string prefix;

        public WebRepositoryBase(IJsonWebService webService, string prefix)
        {
            this.webService = webService;
            this.prefix = prefix;
        }

        public Task Add(T task) => webService.Post(prefix, task);
        public Task Update(T task) => webService.Put(prefix, task);
        public Task Delete(T task) => webService.Delete(prefix+"/"+KeyString(task));
        protected abstract string KeyString(T item);
        public async IAsyncEnumerable<T> ItemsFromKeys(IEnumerable<Guid> keys)
        {
            foreach (var task in await webService.Get<Guid[],IList<T>>($"{prefix}/Query", keys.ToArray()))
            {
                yield return task;
            }
        }

        public async IAsyncEnumerable<T> TasksForDate(LocalDate date, DateTimeZone zone)
        {
            foreach (var task in await webService.Get<List<T>>($"{prefix}/{date:yyyy-MM-dd}/{HttpUtility.UrlEncode(zone.Id)}"))
            {
                yield return task;
            }
        }
    }
}