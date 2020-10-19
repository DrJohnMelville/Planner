using System;
using Melville.INPC;
using NodaTime;
using Planner.Models.Notes;
using Planner.Models.Repositories;

namespace Planner.Models.Blobs
{
    public partial class Blob: PlannerItemWithDate
    {
        [AutoNotify] private Instant timeCreated;
        [AutoNotify] private string name = "";
        [AutoNotify] private string mimeType = "";
    }
}