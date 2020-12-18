using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Planner.Models.Appointments;
using Planner.Models.Repositories;

namespace Planner.Repository.SqLite
{
    public class AppointmentRemoteRepository:IDatedRemoteRepository<Appointment>
    {
        private readonly Func<PlannerDataContext> contextFactory;
        public AppointmentRemoteRepository(Func<PlannerDataContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        // right noow this is quite the refused beqyest.  I am not yet sure if I am going to implement
        // editing events in app, so I will defer any implementation for now.
        public Task Add(Appointment task)
        {
            throw new NotImplementedException();
        }

        public Task Update(Appointment task)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Appointment task)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<Appointment> ItemsFromKeys(IEnumerable<Guid> keys)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<Appointment> TasksForDate(LocalDate date, DateTimeZone timeZone)
        {
            var start = date.AtStartOfDayInZone(timeZone).ToInstant();
            var end = date.PlusDays(1).AtStartOfDayInZone(timeZone).PlusSeconds(-1).ToInstant();
            return SimpleQuery(i => i.Start < end && i.End > start);
        }

        private IAsyncEnumerable<Appointment> SimpleQuery(Expression<Func<Appointment, bool>> predicate)
        {
            var ctx = contextFactory();
            return new DisposeWithAsyncEnumerable<Appointment>(ctx.Set<Appointment>()
                .Include(i=>i.AppointmentDetails)
                .AsNoTracking()
                .Where(predicate)
                .AsAsyncEnumerable(), ctx);
        }

    }
}