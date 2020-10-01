﻿using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Melville.TestHelpers.Http;
using Melville.TestHelpers.MockConstruction;
using Moq;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Planner.Models.Repositories;
using Planner.Repository.Test.SqLite;
using Planner.Repository.Web;
using Xunit;

namespace Planner.Repository.Test.Web
{
    public class PlannerTaskWebRepositoryTest
    {
        private readonly Mock<IHttpClientMock> httpSource = new Mock<IHttpClientMock>();
        private readonly IJsonWebService service;
        private readonly PlannerTaskWebRepository sut;
        private readonly LocalDate date = new LocalDate(1975, 07, 28);
        public PlannerTaskWebRepositoryTest()
        {
            var seropt = new JsonSerializerOptions();
            seropt.IgnoreReadOnlyProperties = true;
            seropt.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            var httpClient = httpSource.ToHttpClient();
           httpClient.BaseAddress = new Uri("https://Planner.DRJohnMelville.com");
            service = new JsonWebService(httpClient, seropt);
            sut = new PlannerTaskWebRepository(service);
        }

        [Fact]
        public async Task GetPlannerTasks()
        {
            httpSource.Setup(i=>i.EndsWith("Task/1975-07-28"), HttpMethod.Get).
                ReturnsJson("[{\"Date\":\"1975-07-28\",\"Name\":\"My Birthday\",\"Priority\":\" \",\"Order\":0,\"Status\":0,\"StatusDetail\":\"\"}," +
                            "{\"Date\":\"1975-07-28\",\"Name\":\"Second Task\",\"Priority\":\" \",\"Order\":0,\"Status\":0,\"StatusDetail\":\"\"}]");
            var list = await sut.TasksForDate(date).ToListAsync();
            Assert.Equal(2, list.Count);
            Assert.Equal("My Birthday", list[0].Name);
            Assert.Equal("Second Task", list[1].Name);
        }

        [Fact]
        public async Task DeleteTask()
        {
            httpSource.Setup(i=>true, HttpMethod.Delete).ReturnsJson("");
            var guid = Guid.NewGuid();
            await sut.DeleteTask(new RemotePlannerTask(guid));
            httpSource.Verify((Func<string,bool>)(i=>i.EndsWith("/Task/"+guid)), 
                HttpMethod.Delete, Times.Once);
        }

        [Fact]
        public async Task PutChangeMethod()
        {
            httpSource.Setup(i=>true, HttpMethod.Put).ReturnsJson("");
            await sut.AddOrUpdateTask(new RemotePlannerTask());
            httpSource.Verify((Func<string,bool>)(i=>i.EndsWith("/Task")), 
                HttpMethod.Put, Times.Once);
        }
    }
}