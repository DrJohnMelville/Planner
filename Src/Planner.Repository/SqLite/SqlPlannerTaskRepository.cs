using System;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Planner.Models.Tasks;

namespace Planner.Repository.SqLite
{
    public class SqlPlannerTasRemoteRepository: SqlRemoteRepositoryWithDate<PlannerTask>, IDatedRemoteRepository<PlannerTask>
    {
        public SqlPlannerTasRemoteRepository(Func<PlannerDataContext> contextFactory) : base(contextFactory)
        {
        }
    }
}