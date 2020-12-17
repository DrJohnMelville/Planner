using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Melville.TestHelpers.Http;
using Moq;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Planner.Repository.Web;

namespace Planner.Repository.Test.Web
{
    public class TestWithJsonWebService
    {
        protected readonly Mock<IHttpClientMock> httpSource = new Mock<IHttpClientMock>();
        protected readonly IJsonWebService service;
        protected readonly LocalDate date = new LocalDate(1975, 07, 28);

        public TestWithJsonWebService()
        {
            var seropt = new JsonSerializerOptions();
            seropt.IgnoreReadOnlyProperties = true;
            seropt.ReferenceHandler = ReferenceHandler.Preserve;
            seropt.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            var httpClient = httpSource.ToHttpClient();
            httpClient.BaseAddress = new Uri("https://Planner.DRJohnMelville.com");
            service = new JsonWebService(httpClient, seropt);
        }
    }
}