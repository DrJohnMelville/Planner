using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Planner.Models.Appointments;
using Planner.Models.Appointments.SuncStructure;
using Planner.Repository.SqLite;
using Xunit;

namespace Planner.Repository.Test.SqLite
{
    public class AppointmentSyncEngineTest
    {
        private readonly TestDatabase db = new TestDatabase();
        private readonly AppointmentSyncEngine sut;

        public AppointmentSyncEngineTest()
        {
            sut = new AppointmentSyncEngine(db.NewContext);
        }

        [Fact]
        public async Task AddItemsTest()
        {
            await sut.Synchronize(new AppointmentSyncInfo()
            {
                Items = new List<SyncAppointmentData>()
                {
                    new()
                    {
                        Title = "Apt1Title",
                        Location = "Apt1Location",
                        BodyText = "Apt1BodyText",
                        UniqueOutlookId = "OutlookId",
                        Times = new List<SyncAppointmentTime>()
                        {
                            new() {StartTime = Instant.MinValue, EndTime = Instant.MaxValue}
                        }
                    }
                }
            });

            var appts = await GetAppointmentsList();
            Assert.Single(appts);
            Assert.Equal("Apt1BodyText", appts[0].AppointmentDetails.BodyText);
            
        }

        private async Task<List<Appointment>> GetAppointmentsList()
        {
            using var ctx = db.NewContext();
            var appts = await ctx.Appointments.Include(i => i.AppointmentDetails).ToListAsync();
            return appts;
        }

        [Fact]
        public async Task Add2Dates()
        {
            await AddMultiAppointment();

            var appts = await GetAppointmentsList();
            Assert.Equal(2, appts.Count);
            Assert.Equal(appts[0].AppointmentDetails, appts[1].AppointmentDetails);
        }
        [Fact]
        public async Task Update()
        {
            await AddMultiAppointment();
            await AddMultiAppointment();

            var appts = await GetAppointmentsList();
            Assert.Equal(2, appts.Count);
            Assert.Equal(appts[0].AppointmentDetails, appts[1].AppointmentDetails);
        }

        private async Task AddMultiAppointment()
        {
            await sut.Synchronize(new AppointmentSyncInfo()
            {
                Items = new List<SyncAppointmentData>()
                {
                    new()
                    {
                        Title = "Apt1Title",
                        Location = "Apt1Location",
                        BodyText = "Apt1BodyText",
                        UniqueOutlookId = "OutlookId",
                        Times = new List<SyncAppointmentTime>()
                        {
                            new() {StartTime = Instant.MinValue, EndTime = Instant.MaxValue},
                            new() {StartTime = Instant.FromUnixTimeTicks(19), EndTime = Instant.MaxValue}
                        }
                    }
                }
            });
        }

        [Fact]
        public async Task DeleteAppointment()
        {
            await AddMultiAppointment();
            await sut.Synchronize(new AppointmentSyncInfo
            {
                KeysToDelete = new List<string>(){"OutlookId"}
            });
            var appts = await GetAppointmentsList();
            Assert.Empty(appts);            
        }
    }
}