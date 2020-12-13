using System.Threading.Tasks;

namespace Planner.Models.Appointments.SyncStructure
{
    public interface IAppointmentSyncEngine
    {
        Task Synchronize(AppointmentSyncInfo info);
    }
}