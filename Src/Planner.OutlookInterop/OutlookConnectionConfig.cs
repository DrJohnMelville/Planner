using System;
using System.Collections.Generic;
using System.Linq;
using Melville.Hacks;
using Planner.Models.Appointments.SyncStructure;

namespace Planner.OutlookInterop
{
    public class OutlookConnectionConfig
    {
        public string Account { get; set; } = "";
    }

    public class AppointmentSyncMonitor : IAppointmentSyncMonitor
    {
        private readonly IList<OutlookConnectionConfig> config;
        private Func<string, OutlookSyncMonitor> factory;
        private readonly List<OutlookSyncMonitor> engines = new();

        public AppointmentSyncMonitor(IList<OutlookConnectionConfig> config, 
            Func<string, OutlookSyncMonitor> factory)
        {
            this.config = config;
            this.factory = factory;
        }

        public void Start()
        {
            engines.AddRange(config.Select(i=>factory(i.Account)));
            foreach (var engine in engines)
            {
                engine.DoSync().FireAndForget();
            }
        }
    }
}