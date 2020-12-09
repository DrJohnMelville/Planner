using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NodaTime;
using Planner.Models.Appointments;
using Planner.Repository.SqLite;
using Xunit;

namespace Planner.Repository.Test.SqLite
{
    public class AppointmentRemoteRepositoryTest
    {
        private readonly TestDatabase data = new();
        private readonly AppointmentRemoteRepository sut;
        private readonly LocalDate date1 = new(1975,07,28);
        private readonly LocalDate date2 = new(1974,08,18);
        private readonly DateTimeZone tzUtc = DateTimeZone.Utc;
        private readonly DateTimeZone tzSix = DateTimeZone.ForOffset(Offset.FromHours(-6));

        public AppointmentRemoteRepositoryTest()
        {
            sut = new AppointmentRemoteRepository(data.NewContext);
            SeedAppointments();
        }

        private void SeedAppointments()
        {
            using var db = data.NewContext();
            var appGuid = Guid.NewGuid();
            db.AppointmentDetails.Add(new AppointmentDetails
            {
                Title = "App Title",
                Location = "App Location",
                BodyText = "Body Text",
                AppointmentDetailsId = appGuid,
                Appointments = new Appointment[]
                {
                    CreateAppointment(appGuid, date1),
                    CreateAppointment(appGuid, date2)
                }
            });
            db.SaveChanges();
        }

        private Appointment CreateAppointment(Guid appGuid, LocalDate date)
        {
            var startTime = date.AtStartOfDayInZone(tzUtc).ToInstant().Plus(Duration.FromHours(2));
            return new() {AppointmentDetailsId = appGuid, 
                Start = startTime, End = startTime.Plus(Duration.FromHours(1))};
        }

        [Fact]
        public async Task ReadAppointmentsForDay()
        {
            var appts = await sut.TasksForDate(date1, DateTimeZone.Utc).ToListAsync();

            Assert.Single(appts);
            var appt = appts.First();
            Assert.Equal("App Title", appt.AppointmentDetails!.Title);
            Assert.Equal("App Location", appt.AppointmentDetails!.Location);
            Assert.Equal("Body Text", appt.AppointmentDetails!.BodyText);
            Assert.Equal(2, appt.Start.InZone(tzUtc).Hour);
            Assert.Equal(20, appt.Start.InZone(tzSix).Hour);
            Assert.Equal(27, appt.Start.InZone(tzSix).Day);
            
        }

        [Fact]
        public async Task AppointmentsShiftInTimeZone()
        {
            Assert.Single(await sut.TasksForDate(date1, tzUtc).ToListAsync());
            Assert.Empty(await sut.TasksForDate(date1, tzSix).ToListAsync());
            Assert.Single(await sut.TasksForDate(date1.PlusDays(-1), tzSix).ToListAsync());
        }

        [Fact]
        public async Task MultipleOccurences()
        {
            Assert.Single(await sut.TasksForDate(date1, tzUtc).ToListAsync());
            Assert.Single(await sut.TasksForDate(date2, tzUtc).ToListAsync());
        }

    }
}