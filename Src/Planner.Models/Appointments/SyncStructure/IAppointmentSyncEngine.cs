using System.Threading.Tasks;
using NodaTime;

namespace Planner.Models.Appointments.SyncStructure
{
    public interface IAppointmentSyncEngine
    {
        Task Synchronize(AppointmentSyncInfo info);
        Task<Instant> LastSynchronizationTime();
        Task ClearAppointments();
    }
}