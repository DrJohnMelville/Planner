﻿using System;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Planner.Models.Tasks;

namespace Planner.Repository.SqLite
{
    public class SqlPlannerTasRemoteRepository: SqlRemoteRepositoryWithDate<PlannerTask>, IPlannerTasRemoteRepository
    {
        public SqlPlannerTasRemoteRepository(Func<PlannerDataContext> contextFactory) : base(contextFactory)
        {
        }
    }

    public class SqlNoteRemoteRepository : SqlRemoteRepositoryWithDate<Note>, INoteRemoteRepository
    {
        public SqlNoteRemoteRepository(Func<PlannerDataContext> contextFactory) : base(contextFactory)
        {
        }
    }
}