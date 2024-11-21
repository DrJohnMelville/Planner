using System.Text.Json;
using System.Text.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Planner.Models.Tasks;

namespace Planner.Test.Models.Tasks;

public class PlannerTaskJsonSerialization
{
    private const string JsonTextForObject = "{\"$id\":\"1\",\"Name\":\"Foo\",\"Priority\":\"A\",\"Order\":2,\"Status\":0,\"StatusDetail\":\"\",\"Date\":\"0001-01-01\",\"Key\":\"f3d68e2f-10ec-4a7c-9de7-bdb26906718b\"}";

    [Test]
    public void Serialize()
    {
        var sut = new PlannerTask(Guid.Parse("f3d68e2f-10ec-4a7c-9de7-bdb26906718b"))
        {
            Name = "Foo",
            Priority = 'A',
            Order = 2
        };

        var ser = JsonSerializer.Serialize(sut, SerializerSettings());
        Assert.Equal(JsonTextForObject,ser);
    }

    [Test]
    public void Deserialize()
    {
        var rpt = JsonSerializer.Deserialize<PlannerTask>(
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
        sett.ReferenceHandler = ReferenceHandler.Preserve;
        ;
        return sett;
    }
}