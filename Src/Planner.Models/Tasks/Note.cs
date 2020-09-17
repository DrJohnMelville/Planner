using NodaTime;
using Melville.INPC;

namespace Planner.Models.Tasks
{
    public partial class Note
    {
        [AutoNotify] private LocalDate date;
        [AutoNotify] private int index;
        [AutoNotify] private string name = "";
        [AutoNotify] private string rtfText = "";
    }
}