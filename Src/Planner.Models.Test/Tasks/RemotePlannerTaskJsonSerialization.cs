using System;
using System.Text.Json;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Planner.Models.Repositories;
using Xunit;

namespace Planner.Models.Test.Tasks
{
    public class RemotePlannerTaskJsonSerialization
    {
        private const string JsonTextForObject = "{\"Key\":\"f3d68e2f-10ec-4a7c-9de7-bdb26906718b\",\"Date\":\"0001-01-01\",\"Name\":\"Foo\",\"Priority\":\"A\",\"Order\":2,\"Status\":0,\"StatusDetail\":\"\"}";

        [Fact]
        public void Serialize()
        {
            var sut = new RemotePlannerTask(Guid.Parse("f3d68e2f-10ec-4a7c-9de7-bdb26906718b"))
            {
                Name = "Foo",
                Priority = 'A',
                Order = 2
            };

            var ser = JsonSerializer.Serialize(sut, SerializerSettings());
            Assert.Equal(JsonTextForObject,ser);
        }

        [Fact]
        public void Deserialize()
        {
            var rpt = JsonSerializer.Deserialize<RemotePlannerTask>(
                JsonTextForObject,
                SerializerSettings());
            Assert.Equal(Guid.Parse("f3d68e2f-10ec-4a7c-9de7-bdb26906718b"), rpt.Key);
            Assert.Equal("Foo", rpt.Name);
            Assert.Equal(2, rpt.Order);
            Assert.Equal('A', rpt.Priority);
            
        }

        private static JsonSerializerOptions SerializerSettings()
        {
            var sett = new JsonSerializerOptions();
            sett.IgnoreReadOnlyProperties = true;
            sett.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            return sett;
        }
    }
}